// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Allows a linear traversel from every item in this collection up to every item
        /// that matches the provided selector
        /// </summary>
        /// <param name="predicate">A selector to detect child items</param>
        public static IEnumerable<T> Traverse<T>(this T[] items, Func<T, IEnumerable<T>> predicate)
        {
            Stack<T> stack = StackPool<T>.Get();
            try
            {
                for (int i = 0; i < items.Length; i++)
                {
                    stack.Push(items[i]);
                    while (stack.Count > 0)
                    {
                        T next = stack.Pop();
                        if (next != null)
                        {
                            yield return next;
                            foreach (T item in predicate(next))
                                stack.Push(item);
                        }
                    }
                }
            }
            finally
            {
                StackPool<T>.Return(stack);
            }
        }
    }
}
