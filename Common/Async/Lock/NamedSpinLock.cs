// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading
{
    /// <summary>
    /// Spin awaits until access is passed
    /// </summary>
    public sealed class NamedSpinlock : FinalizerObject
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

            public void Lock()
            {
                while (@lock.Exchange(1) != 0)
                {
                    while (@lock != 0)
                    { }
                }
            }
            public bool TryGetLock()
            {
                return (@lock.CompareExchange(1, 0) == 0);
            }

            public void Release()
            {
                @lock.Exchange(0);
            }
        }

        const int MaxDelay = 100;

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
        public NamedSpinlock(string id)
        {
            LockTable.TryGetValue(id, out instance);
            this.id = id;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LockTable.Remove(id);
                instance.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Try to acquire access to the critical section
        /// </summary>
        public void Lock()
        {
            instance.Lock();
        }
        /// <summary>
        /// Try to acquire access to the critical section
        /// </summary>
        public async Task LockAsync()
        {
            for (int i = 0; !instance.TryGetLock();)
            {
                await Taskʾ.Delay(i);
                if (i < MaxDelay)
                    i++;
            }
        }

        /// <summary>
        /// Passes access for this lock object back
        /// </summary>
        public void Release()
        {
            instance.Release();
        }
    }
}
