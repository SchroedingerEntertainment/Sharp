// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public static class Fiber
    {
        private static FiberLocal<Int32> idStore;
        private static atomic_int nextId;

        /// <summary>
        /// 
        /// </summary>
        public static Int32 Id
        {
            get
            {
                Int32 id; if (!idStore.TryGet(out id))
                {
                    id = nextId.Increment();
                    idStore.Value = id;
                }
                return id;
            }
        }

        static Fiber()
        {
            idStore = new FiberLocal<Int32>();
        }
    }
}