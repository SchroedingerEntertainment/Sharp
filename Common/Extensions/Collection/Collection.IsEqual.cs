// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Compares items in one data vector to equality of items in another data vector
        /// regardless of their order
        /// </summary>
        /// <param name="comparants">A data vector to compare for equality</param>
        /// <returns>True if both data vector contain the same elements, false otherwise</returns>
        public static bool IsEqual<T>(this ICollection<T> items, ICollection<T> comparants)
        {
            if (items == null || comparants == null || items.Count != comparants.Count)
                return false;
            else if (items.Count == 0)
                return true;

            Dictionary<T, int> lookUp = new Dictionary<T, int>();
            foreach(T item in items)
            {
                int count; if (!lookUp.TryGetValue(item, out count))
                {
                    lookUp.Add(item, 1);
                    continue;
                }
                lookUp[item] = count + 1;
            }
            foreach (T item in comparants)
            {
                int count; if (!lookUp.TryGetValue(item, out count))
                    return false;

                count--;

                if (count <= 0) lookUp.Remove(item);
                else lookUp[item] = count;
            }
            return lookUp.Count == 0;
        }
        /// <summary>
        /// Compares items in one data vector to equality of items in another data vector
        /// regardless of their order
        /// </summary>
        /// <param name="comparants">A data vector to compare for equality</param>
        /// <returns>True if both data vector contain the same elements, false otherwise</returns>
        public static bool IsEqual<T>(this ICollection<T> items, ICollection<T> comparants, IEqualityComparer<T> comparer)
        {
            if (items == null || comparants == null || items.Count != comparants.Count)
                return false;
            else if (items.Count == 0)
                return true;

            Dictionary<T, int> lookUp = new Dictionary<T, int>(comparer);
            foreach (T item in items)
            {
                int count; if (!lookUp.TryGetValue(item, out count))
                {
                    lookUp.Add(item, 1);
                    continue;
                }
                lookUp[item] = count + 1;
            }
            foreach (T item in comparants)
            {
                int count; if (!lookUp.TryGetValue(item, out count))
                    return false;

                count--;

                if (count <= 0) lookUp.Remove(item);
                else lookUp[item] = count;
            }
            return lookUp.Count == 0;
        }
    }
}
