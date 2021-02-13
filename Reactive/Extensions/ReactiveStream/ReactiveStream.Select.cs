// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class ReactiveStreamExtension
    {
        /// <summary>
        /// Projects each element of the stream to another stream
        /// </summary>
        public static IReactiveStream<OutSource, Result> Select<InSource, Result, OutSource>(this IReactiveStream<InSource, Result> stream, Func<InSource, OutSource> selector)
        {
            return new WhereSelectStream<InSource, Result, OutSource> (stream, selector);
        }
    }
}