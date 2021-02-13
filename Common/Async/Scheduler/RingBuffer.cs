// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading
{
    /// <summary>
    /// A thread-safe continous object buffer
    /// </summary>
    public class RingBuffer<T> where T : class
    {
        private const int EnqueueThreshold = 20; //CPU cycles
        private const int DequeueThreshold = 10; //CPU cycles

        T[] buffer;
        int mask;
        int size;

        Spinlockʾ readLock;
        int tail;

        Spinlockʾ writeLock;
        int head;

        /// <summary>
        /// The amount of items that can be stored
        /// </summary>
        public int Capacity
        {
            get
            {
                return buffer.Length;
            }
        }

        /// <summary>
        /// The amount of items currently stored
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
        }

        /// <summary>
        /// Creates a new buffer instance with the provided capacity
        /// </summary>
        /// <param name="capacity">A capacity value that must be greater than zero and power-of-two</param>
        public RingBuffer(int capacity)
        {
            this.buffer = new T[capacity.NextPowerOfTwo()];
            this.writeLock = new Spinlockʾ();
            this.readLock = new Spinlockʾ();
            this.mask = buffer.Length - 1;
        }

        /// <summary>
        /// Tries to enqueue an item to the buffer if possible
        /// </summary>
        /// <param name="item">The item to be stored</param>
        /// <returns>True if an item was enqueued successfully, false otherwise</returns>
        public bool Enqueue(T item)
        {
            writeLock.Lock();
            try
            {
                for (int i = 0; Interlocked.CompareExchange<T>(ref buffer[head], item, null) != null; i++)
                {
                    if (i >= EnqueueThreshold)
                        return false;
                }
                head = ((head + 1) & mask);
            }
            finally
            {
                writeLock.Release();
            }
            Interlocked.Increment(ref size);
            return true;
        }

        /// <summary>
        /// Tries to dequeue an item from the buffer if possible
        /// </summary>
        /// <param name="item">The resulting item to be read into</param>
        /// <returns>True if an item was dequeued successfully, false otherwise</returns>
        public bool Dequeue(ref T item)
        {
            readLock.Lock();
            try
            {
                item = buffer[tail];
                for (int i = 0; Interlocked.CompareExchange<T>(ref buffer[tail], null, item) == null || item == null; i++)
                {
                    if (size > 0 && i < DequeueThreshold)
                    {
                        item = buffer[tail];
                    }
                    else return false;
                }
            }
            finally
            {
                readLock.Release();
            }
            tail = ((tail + 1) & mask);
            Interlocked.Decrement(ref size);
            return true;
        }
    }
}
