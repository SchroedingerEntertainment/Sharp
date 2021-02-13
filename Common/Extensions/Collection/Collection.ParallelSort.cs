// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static Task ParallelSort<T>(this List<T> items, IComparer<T> comparer)
        {
            return Quicksort.ParallelSort(items, 0, items.Count - 1, comparer);
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static Task ParallelSort<T>(this List<T> items)
        {
            return Quicksort.ParallelSort(items, 0, items.Count - 1, Comparer<T>.Default);
        }
    }
}
