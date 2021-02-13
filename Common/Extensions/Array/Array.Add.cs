// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Adds the provided element to the end of this array
        /// </summary>
        /// <returns>The modified array pointer</returns>
        public static T[] Add<T>(this T[] arr, T element)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.GetUpperBound(0)] = element;
            return arr;
        }
    }
}
