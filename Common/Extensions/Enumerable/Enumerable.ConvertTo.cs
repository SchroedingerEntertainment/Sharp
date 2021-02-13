// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static ICollection<TOut> ConvertTo<TIn, TOut>(this IEnumerable<TIn> items, Converter<TIn, TOut> converter)
        {
            List<TOut> result = new List<TOut>();
            IEnumerator<TIn> iterator = items.GetEnumerator();

            for (int i = 0; iterator.MoveNext(); i++)
                result.Add(converter(iterator.Current));

            return result;
        }
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="result">An array to assign converted values into</param>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static void ConvertTo<TIn, TOut>(this IEnumerable<TIn> items, TOut[] result, Converter<TIn, TOut> converter)
        {
            IEnumerator<TIn> iterator = items.GetEnumerator();
            for (int i = 0; i < result.Length && iterator.MoveNext(); i++)
                result[i] = converter(iterator.Current);
        }
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="result">An array to assign converted values into</param>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static void ConvertTo<TIn, TOut>(this IEnumerable<TIn> items, ICollection<TOut> result, Converter<TIn, TOut> converter)
        {
            IEnumerator<TIn> iterator = items.GetEnumerator();
            for (int i = 0; iterator.MoveNext(); i++)
                result.Add(converter(iterator.Current));
        }
    }
}
