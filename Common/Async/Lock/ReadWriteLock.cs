// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading
{
    /// <summary>
    /// Spin awaits until desired access is passed
    /// </summary>
    public struct ReadWriteLock
    {
        public const Int32 WriteMask = 0x7ffffff;
        public const Int32 ReaderMask = 0x8000000;
        const int MaxDelay = 1000;

        atomic_int @lock;
        /// <summary>
        /// Gets current lock state
        /// </summary>
        public Int32 State
        {
            get { return @lock.Value; }
        }

        Int32 scopeId;

        atomic_int writeReferences;
        /// <summary>
        /// Gets the amount of write access nesting in this scope
        /// </summary>
        public int RefCount
        {
            get { return writeReferences.Value; }
        }

        /// <summary>
        /// Try to acquire inclusive read access to the critical section
        /// </summary>
        public void ReadLock()
        {
            for (;;)
            {
                // Wait until there's no active writer
                while ((@lock & ReaderMask) != 0)
                    Thread.Sleep(0);

                Int32 oldLock = (@lock & WriteMask);
                Int32 newLock = oldLock + 1;

                if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                    return;
            }
        }

        /// <summary>
        /// Try to acquire inclusive read access to the critical section
        /// </summary>
        /// <returns>True if successfully locked, false otherwise</returns>
        public bool TryGetReadLock()
        {
            // Wait until there's no active writer
            if ((@lock & ReaderMask) != 0)
                return false;

            Int32 oldLock = (@lock & WriteMask);
            Int32 newLock = oldLock + 1;

            return (@lock.CompareExchange(newLock, oldLock) == oldLock);
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void ReadRelease()
        {
            @lock.Decrement();
        }

        /// <summary>
        /// Try to acquire exclusive write access to the critical section
        /// </summary>
        public void WriteLock()
        {
            if (scopeId == Thread.CurrentThread.ManagedThreadId)
            {
                if ((@lock & WriteMask) != 0)
                {
                    throw new SynchronizationLockException();
                }
                writeReferences.Increment();
                return;
            }
            for (;;)
            {
                if (scopeId == Thread.CurrentThread.ManagedThreadId)
                {
                    writeReferences.Increment();
                    return;
                }

                // Wait until there's no active writer
                while ((@lock & ReaderMask) != 0)
                    Thread.Sleep(0);

                Int32 oldLock = (@lock & WriteMask);
                Int32 newLock = (oldLock | ReaderMask);

                if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                {
                    // Wait for active readers to release their locks
                    while ((@lock & WriteMask) != 0)
                        Thread.Sleep(0);

                    scopeId = Thread.CurrentThread.ManagedThreadId;
                    return;
                }
            }
        }

        /// <summary>
        /// Try to acquire write access to the critical section
        /// </summary>
        /// <returns>True if successfully locked, false otherwise</returns>
        public bool TryGetWriteLock()
        {
            if (scopeId == Thread.CurrentThread.ManagedThreadId && (@lock & WriteMask) == 0)
            {
                if ((@lock & WriteMask) != 0)
                {
                    throw new SynchronizationLockException();
                }
                writeReferences.Increment();
                return true;
            }

            // Wait until there's no active writer
            if ((@lock & ReaderMask) != 0)
                return false;

            Int32 oldLock = (@lock & WriteMask);
            Int32 newLock = (oldLock | ReaderMask);

            if (@lock.CompareExchange(newLock, oldLock) == oldLock)
            {
                // Wait for active readers to release their locks
                while ((@lock & WriteMask) != 0)
                {
                    if (@lock.CompareExchange(oldLock, newLock) == newLock)
                        return false;
                }

                scopeId = Thread.CurrentThread.ManagedThreadId;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void WriteRelease()
        {
            if (scopeId != Thread.CurrentThread.ManagedThreadId)
            {
                throw new UnauthorizedAccessException();
            }
            if (writeReferences > 0) writeReferences.Decrement();
            else
            {
                scopeId = 0;
                @lock.Exchange(0);
            }
        }
    }
}