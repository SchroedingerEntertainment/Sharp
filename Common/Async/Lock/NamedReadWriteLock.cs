// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading
{
    /// <summary>
    /// Spin awaits until desired access is passed
    /// </summary>
    public sealed class NamedReadWriteLock : FinalizerObject
    {
        public static class LockTable
        {
            private static Dictionary<string, LockObject> locks;
            private static Spinlockʾ @lock;

            static LockTable()
            {
                locks = new Dictionary<string, LockObject>();
                @lock = new Spinlockʾ();
            }

            public static bool TryGetValue(string key, out LockObject value)
            {
                @lock.Lock();
                try
                {
                    if (!locks.TryGetValue(key, out value))
                    {
                        value = GenericObjectPool<LockObject>.Get();
                        locks.Add(key, value);
                    }
                    value.RefCount++;
                    return true;
                }
                finally
                {
                    @lock.Release();
                }
            }
            public static bool Remove(string key)
            {
                @lock.Lock();
                try
                {
                    LockObject value;
                    if (locks.TryGetValue(key, out value))
                    {
                        value.RefCount--;
                        if (value.RefCount <= 0 && locks.Remove(key))
                        {
                            if (value.State != 0)
                            {
                                throw new SynchronizationLockException();
                            }
                            GenericObjectPool<LockObject>.Return(value);
                        }
                    }
                    return false;
                }
                finally
                {
                    @lock.Release();
                }
            }
        }
        public class LockObject : FinalizerObject
        {
            atomic_int @lock;
            public Int32 State
            {
                get { return @lock.Value; }
            }

            int references;
            public Int32 RefCount
            {
                get { return references; }
                internal set { references = value; }
            }
            
            public LockObject()
            { }
            protected override void Dispose(bool disposing)
            {
                if (@lock != 0)
                {
                    throw new SynchronizationLockException();
                }
                base.Dispose(disposing);
            }

            public void ReadLock()
            {
                for (;;)
                {
                    while ((@lock & ReadWriteLock.ReaderMask) != 0)
                        Thread.Sleep(0);

                    Int32 oldLock = (@lock & ReadWriteLock.WriteMask);
                    Int32 newLock = oldLock + 1;

                    if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                        return;
                }
            }
            public bool TryGetReadLock()
            {
                if ((@lock & ReadWriteLock.ReaderMask) != 0)
                    return false;

                Int32 oldLock = (@lock & ReadWriteLock.WriteMask);
                Int32 newLock = oldLock + 1;

                return (@lock.CompareExchange(newLock, oldLock) == oldLock);
            }

            public void ReadRelease()
            {
                for (;;)
                {
                    Int32 oldLock = (@lock & ReadWriteLock.WriteMask);
                    Int32 newLock = oldLock - 1;

                    if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                        return;
                }
            }

            public void WriteLock()
            {
                for (;;)
                {
                    while ((@lock & ReadWriteLock.ReaderMask) != 0)
                        Thread.Sleep(0);

                    Int32 oldLock = (@lock & ReadWriteLock.WriteMask);
                    Int32 newLock = (oldLock | ReadWriteLock.ReaderMask);

                    if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                    {
                        while ((@lock & ReadWriteLock.WriteMask) != 0)
                            Thread.Sleep(0);

                        return;
                    }
                }
            }
            public bool TryGetWriteLock()
            {
                if ((@lock & ReadWriteLock.ReaderMask) != 0)
                    return false;

                Int32 oldLock = (@lock & ReadWriteLock.WriteMask);
                Int32 newLock = (oldLock | ReadWriteLock.ReaderMask);

                if (@lock.CompareExchange(newLock, oldLock) == oldLock)
                {
                    while ((@lock & ReadWriteLock.WriteMask) != 0)
                    {
                        if (@lock.CompareExchange(oldLock, newLock) == newLock)
                            return false;
                    }
                    return true;
                }
                else return false;
            }

            public void WriteRelease()
            {
                @lock.Exchange(0);
            }
        }

        const int MaxDelay = 1000;

        LockObject instance;
        string id;

        /// <summary>
        /// Gets current lock state
        /// </summary>
        public Int32 State
        {
            get { return instance.State; }
        }
        /// <summary>
        /// Gets the amount of nesting in this scope
        /// </summary>
        public Int32 RefCount
        {
            get { return instance.RefCount; }
        }

        /// <summary>
        /// Creates a new or obtains an existing lock instance
        /// </summary>
        /// <param name="id">An ID to associate this lock-object with</param>
        public NamedReadWriteLock(string id)
        {
            LockTable.TryGetValue(id, out instance);
            this.id = id;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LockTable.Remove(id);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Try to acquire inclusive read access to the critical section
        /// </summary>
        public void ReadLock()
        {
            instance.ReadLock();
        }
        /// <summary>
        /// Try to acquire inclusive read access to the critical section
        /// </summary>
        public async Task ReadLockAsync()
        {
            for (int i = 0; !instance.TryGetReadLock();)
            {
                await Taskʾ.Delay(i);
                if (i < MaxDelay)
                    i++;
            }
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void ReadRelease()
        {
            instance.ReadRelease();
        }

        /// <summary>
        /// Try to acquire exclusive write access to the critical section
        /// </summary>
        public void WriteLock()
        {
            instance.WriteLock();
        }
        /// <summary>
        /// Try to acquire exclusive write access to the critical section
        /// </summary>
        public async Task WriteLockAsync()
        {
            for (int i = 0; !instance.TryGetWriteLock();)
            {
                await Taskʾ.Delay(i);
                if (i < MaxDelay)
                    i++;
            }
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void WriteRelease()
        {
            instance.WriteRelease();
        }
    }
}