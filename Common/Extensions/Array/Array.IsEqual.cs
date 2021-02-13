// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Compares items in one data vector to equality of items in another data vector
        /// regardless of their order
        /// </summary>
        /// <param name="comparants">A data vector to compare for equality</param>
        /// <returns>True if both data vector contain the same elements, false otherwise</returns>
        public static bool IsEqual<T>(this T[] items, T[] comparants)
        {
            if (items == null || comparants == null || items.Length != comparants.Length)
            {
                return false;
            }
            else if (items.Length == 0)
            {
                return true;
            }

            int length = items.Length;
            Dictionary<T, int> lookUp = CollectionPool<Dictionary<T, int>, T, int>.Get();
            try
            {
                for (int i = 0; i < length; i++)
                {
                    int count; if (!lookUp.TryGetValue(items[i], out count))
                    {
                        lookUp.Add(items[i], 1);
                        continue;
                    }
                    lookUp[items[i]] = count + 1;
                }
                for (int i = 0; i < length; i++)
                {
                    int count; if (!lookUp.TryGetValue(comparants[i], out count))
                        return false;

                    count--;

                    if (count <= 0) lookUp.Remove(comparants[i]);
                    else lookUp[comparants[i]] = count;
                }
                return lookUp.Count == 0;
            }
            finally
            {
                CollectionPool<Dictionary<T, int>, T, int>.Return(lookUp);
            }
        }
        /// <summary>
        /// Compares items in one data vector to equality of items in another data vector
        /// regardless of their order
        /// </summary>
        /// <param name="comparants">A data vector to compare for equality</param>
        /// <returns>True if both data vector contain the same elements, false otherwise</returns>
        public static bool IsEqual<T>(this T[] items, T[] comparants, IEqualityComparer<T> comparer)
        {
            if (items == null || comparants == null || items.Length != comparants.Length)
            {
                return false;
            }
            else if (items.Length == 0)
            {
                return true;
            }

            int length = items.Length;
            Dictionary<T, int> lookUp = new Dictionary<T, int>(comparer);
            for (int i = 0; i < length; i++)
            {
                int count; if (!lookUp.TryGetValue(items[i], out count))
                {
                    lookUp.Add(items[i], 1);
                    continue;
                }
                lookUp[items[i]] = count + 1;
            }
            for (int i = 0; i < length; i++)
            {
                int count; if (!lookUp.TryGetValue(comparants[i], out count))
                    return false;

                count--;

                if (count <= 0) lookUp.Remove(comparants[i]);
                else lookUp[comparants[i]] = count;
            }
            return (lookUp.Count == 0);
        }
    }
}
