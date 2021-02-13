// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
    public static class Quicksort
    {
        private const int SequentialThreshold = 2048;

        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static void Sort<T>(T[] items, int left, int right, IComparer<T> comparer)
        {
            if (right > left)
            {
                int pivot = Partition(items, left, right, comparer);
                Sort(items, left, pivot - 1, comparer);
                Sort(items, pivot + 1, right, comparer);
            }
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static void Sort<T>(List<T> items, int left, int right, IComparer<T> comparer)
        {
            if (right > left)
            {
                int pivot = Partition(items, left, right, comparer);
                Sort(items, left, pivot - 1, comparer);
                Sort(items, pivot + 1, right, comparer);
            }
        }
        
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static async Task ParallelSort<T>(T[] items, int left, int right, IComparer<T> comparer)
        {
            if (right > left)
            {
                if (right - left >= SequentialThreshold)
                {
                    int pivot = Partition(items, left, right, comparer);
                    await ParallelSort<T>(items, left, pivot - 1, comparer);
                    await ParallelSort<T>(items, pivot + 1, right, comparer);
                }
                else Sort(items, left, right, comparer);
            }
        }
        /// <summary>
        /// Sorts items in the given data vector
        /// </summary>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        public static async Task ParallelSort<T>(List<T> items, int left, int right, IComparer<T> comparer)
        {
            if (right > left)
            {
                if (right - left >= SequentialThreshold)
                {
                    int pivot = Partition(items, left, right, comparer);
                    await ParallelSort<T>(items, left, pivot - 1, comparer);
                    await ParallelSort<T>(items, pivot + 1, right, comparer);
                }
                else Sort(items, left, right, comparer);
            }
        }

        /// <summary>
        /// Sorts a list of elements to order
        /// </summary>
        /// <param name="items">The list of elements to sort</param>
        /// <param name="low">The start index of the range to csort</param>
        /// <param name="high">The end index of the range to sort</param>
        /// <returns>The greatest index found in the range</returns>
        public static int Partition<T>(T[] items, int low, int high, IComparer<T> comparer)
        {
            int pivotPos = (high + low) / 2;

            T pivot = items[pivotPos];
            items.Swap(low, pivotPos);

            int left = low;
            for (int i = low + 1; i <= high; i++)
            {
                if (comparer.Compare(items[i], pivot) < 0)
                {
                    left++;
                    items.Swap(i, left);
                }
            }

            items.Swap(low, left);
            return left;
        }
        /// <summary>
        /// Sorts a list of elements to order
        /// </summary>
        /// <param name="items">The list of elements to sort</param>
        /// <param name="left">The start index of the range to csort</param>
        /// <param name="right">The end index of the range to sort</param>
        /// <returns>The greatest index found in the range</returns>
        public static int Partition<T>(List<T> items, int left, int right, IComparer<T> comparer)
        {
            int pivotPos = (right + left) / 2;

            T pivot = items[pivotPos];
            items.Swap(left, pivotPos);

            int last = left;
            for (int i = left + 1; i <= right; i++)
            {
                if (comparer.Compare(items[i], pivot) < 0)
                {
                    last++;
                    items.Swap(i, last);
                }
            }

            items.Swap(left, last);
            return last;
        }
    }
}