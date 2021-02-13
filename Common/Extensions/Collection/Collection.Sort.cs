// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static List<T> Sort<T>(this List<T> items)
        {
            Quicksort.Sort(items, 0, items.Count - 1, Comparer<T>.Default);
            return items;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static List<T> Sort<T>(this List<T> items, IComparer<T> comparer)
        {
            Quicksort.Sort(items, 0, items.Count - 1, comparer);
            return items;
        }
    }
}