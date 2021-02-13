// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Inserts the provided element at index into this array
        /// </summary>
        /// <returns>The modified array pointer</returns>
        public static T[] Insert<T>(this T[] arr, int index, T element)
        {
            int length = arr.Length;
            if (length + 1 <= index)
            {
                throw new IndexOutOfRangeException();
            }

            Array.Resize(ref arr, length + 1);
            Array.Copy(arr, index, arr, index + 1, length - index);
            arr[index] = element;
            return arr;
        }
    }
}
