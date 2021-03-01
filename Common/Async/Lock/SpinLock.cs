// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading
{
    /// <summary>
    /// Spin awaits until access is passed
    /// </summary>
    public struct Spinlockʾ
    {
        const int MaxDelay = 100;

        atomic_int @lock;
        /// <summary>
        /// Gets current lock state
        /// </summary>
        public Int32 State
        {
            get { return @lock.Value; }
        }

        Int32 scopeId;

        atomic_int references;
        /// <summary>
        /// Gets the amount of nesting in this scope
        /// </summary>
        public Int32 RefCount
        {
            get { return references.Value; }
        }

        /// <summary>
        /// Try to acquire access to the critical section
        /// </summary>
        public void Lock()
        {
            if (scopeId == Thread.CurrentThread.ManagedThreadId)
            {
                references.Increment();
                return;
            }

            while (@lock.Exchange(1) != 0)
            {
                while (@lock != 0)
                { }
            }

            references.Increment();
            scopeId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Try to acquire access to the critical section
        /// </summary>
        /// <returns>True if successfully locked, false otherwise</returns>
        public bool TryGetLock()
        {
            return (@lock.CompareExchange(1, 0) == 0);
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void Release()
        {
            if (scopeId != Thread.CurrentThread.ManagedThreadId)
            {
                throw new SynchronizationLockException();
            }
            else if (references.Decrement() == 0)
            {
                scopeId = 0;
                @lock.Exchange(0);
            }
        }
    }
}
