// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static T[] Sort<T>(this T[] items)
        {
            Quicksort.Sort(items, 0, items.Length - 1, Comparer<T>.Default);
            return items;
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        public static T[] Sort<T>(this T[] items, IComparer<T> comparer)
        {
            Quicksort.Sort(items, 0, items.Length - 1, comparer);
            return items;
        }
    }
}