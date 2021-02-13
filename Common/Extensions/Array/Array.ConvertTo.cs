// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static TOut[] ConvertTo<TIn, TOut>(this TIn[] items, Converter<TIn, TOut> converter)
        {
            return Array.ConvertAll<TIn, TOut>(items, converter);
        }
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="result">An array to assign converted values into</param>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static void ConvertTo<TIn, TOut>(this TIn[] items, TOut[] result, Converter<TIn, TOut> converter)
        {
            int length = Math.Min(items.Length, result.Length);
            for (int i = 0; i < length; i++)
                result[i] = converter(items[i]);
        }
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="result">An array to assign converted values into</param>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static void ConvertTo<TIn, TOut>(this TIn[] items, ICollection<TOut> result, Converter<TIn, TOut> converter)
        {
            for (int i = 0; i < items.Length; i++)
                result.Add(converter(items[i]));
        }
    }
}
