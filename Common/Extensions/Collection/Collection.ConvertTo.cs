// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static partial class CollectionExtension
    {
        /// <summary>
        /// Converts the source set into a set of destination values along the provided converter function
        /// </summary>
        /// <param name="converter">A function that provides transform from one type into another</param>
        /// <returns>The resulting set of converted values</returns>
        public static ICollection<TOut> ConvertTo<TIn, TOut>(this ICollection<TIn> items, Converter<TIn, TOut> converter)
        {
            return (items as IEnumerable<TIn>).ConvertTo<TIn, TOut>(converter);
        }
    }
}
