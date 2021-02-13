﻿// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class ObservableExtension
    {
        /// <summary>
        /// Projects each element of the stream to another stream
        /// </summary>
        public static IReactiveStream<TResult> Select<TSource, TResult>(this IObservable<TSource> stream, Func<TSource, TResult> selector)
        {
            return new WhereSelectStream<TSource, TResult>(stream, selector);
        }
    }
}
