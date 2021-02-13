// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a stream instance into a buffered streaming interface
    /// </summary>
    public class StreamedReceiver<T> : IReceiver<T>, IDisposable
    {
        List<T> buffer;
        /// <summary>
        /// The underlaying data buffer
        /// </summary>
        public List<T> Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// Gets the length in bytes of the stream
        /// </summary>
        public long Length
        {
            get { return buffer.Count; }
        }

        long position;
        /// <summary>
        /// Gets or sets the position within the current stream
        /// </summary>
        public long Position
        {
            get { return position; }
            set
            {
                if (value < 0) position = 0;
                else if (value > buffer.Count) position = buffer.Count;
                else position = value;
            }
        }

        /// <summary>
        /// An element considered as currently pointing to
        /// </summary>
        public T Current
        {
            get
            {
                if (position >= buffer.Count) return default(T);
                else return buffer[(int)position];
            }
            set { buffer[(int)position] = value; }
        }

        /// <summary>
        /// The head element of the buffer
        /// </summary>
        public T Head
        {
            get
            {
                if (buffer.Count == 0) return default(T);
                else return buffer[buffer.Count - 1];
            }
        }

        /// <summary>
        /// Creates a new buffer with provided capacity
        /// </summary>
        /// <param name="capacity">The capacity in elements the buffer should manage</param>
        public StreamedReceiver(int capacity)
        {
            this.buffer = new List<T>(capacity);
        }
        /// <summary>
        /// Creates a new buffer instance
        /// </summary>
        public StreamedReceiver()
            :this(16)
        { }
        public virtual void Dispose()
        {
            buffer.Clear();
        }

        public virtual void OnNext(T value)
        {
            buffer.Add(value);
        }
        public virtual void OnError(Exception error)
        { }
        public virtual void OnCompleted()
        { }

        /// <summary>
        /// Determines if the buffer is at the end of data
        /// </summary>
        /// <returns>True if the buffer's end is reached, false otherwise</returns>
        public virtual bool Eof()
        {
            return (Length <= Position);
        }

        /// <summary>
        /// Returns the next element from the buffer if possible
        /// </summary>
        /// <returns>The token read from the buffer or default</returns>
        public T Get()
        {
            if (!Eof())
            {
                T result = buffer[(int)(position)];
                Position++;

                return result;
            }
            else return default(T);
        }

        /// <summary>
        /// Returns the next element from the buffer without processing it if possible
        /// </summary>
        /// <returns>The token read from the buffer or default</returns>
        public T Peek()
        {
            if (!Eof())
            {
                return buffer[(int)(position)];
            }
            else return default(T);
        }

        /// <summary>
        /// Replaces the last item added to the buffer
        /// </summary>
        /// <param name="value">The value to replace the item with</param>
        /// <returns>The value that has been replaced</returns>
        public T Replace(T value)
        {
            if (buffer.Count > 0)
            {
                T result = buffer[buffer.Count - 1];
                buffer[buffer.Count - 1] = value;

                return result;
            }
            else
            {
                buffer.Add(value);
                position++;

                return default(T);
            }
        }

        /// <summary>
        /// Discards items from the end of the buffer
        /// </summary>
        /// <param name="count">An amount of items to discard</param>
        public void Discard(int count)
        {
            buffer.RemoveRange(buffer.Count - count, count);
            position = buffer.Count;
        }

        /// <summary>
        /// Clears the data already read from the buffer and resets the pointer
        /// </summary>
        public void Flush()
        {
            buffer.RemoveRange(0, (int)position);
            position = 0;
        }
    }
}
