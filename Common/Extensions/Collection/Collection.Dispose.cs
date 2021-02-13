// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Performs the Dispose operation on each item in this collection
        /// </summary>
        public static void Dispose<T>(this List<T> items) where T : IDisposable
        {
            foreach (IDisposable item in items)
                item.Dispose();
        }
    }
}
