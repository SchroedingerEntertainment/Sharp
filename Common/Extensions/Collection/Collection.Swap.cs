// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Swaps two items at the given position with each other
        /// </summary>
        /// <returns>The modified array</returns>
        public static List<T> Swap<T>(this List<T> items, int lhs, int rhs)
        {
            T onHold = items[lhs];
            items[lhs] = items[rhs];
            items[rhs] = onHold;

            return items;
        }

        /// <summary>
        /// Swaps the item at given position with last item in the vector and removes it
        /// </summary>
        /// <returns>The item at index</returns>
        public static T SwapRemove<T>(this List<T> items, int index)
        {
            int last = items.Count - 1;

            T onHold = items[index];
            items[index] = items[last];

            items.RemoveAt(last);
            return onHold;
        }
    }
}
