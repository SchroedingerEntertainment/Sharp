// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Swaps two items at the given position with each other
        /// </summary>
        /// <returns>The modified array</returns>
        public static T[] Swap<T>(this T[] items, int lhs, int rhs)
        {
            T onHold = items[lhs];
            items[lhs] = items[rhs];
            items[rhs] = onHold;

            return items;
        }
    }
}
