// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class ReactiveStreamExtension
    {
        /// <summary>
        /// Projects elements of the stream to another stream
        /// </summary>
        public static IReactiveStream<T> Reduce<T, Result>(this IReactiveStream<T, Result> stream, Func<T, Result> predicate)
        {
            return new PredicateStream<T, Result>(stream, predicate);
        }
    }
}