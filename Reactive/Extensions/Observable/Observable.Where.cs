// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class ObservableExtension
    {
        /// <summary>
        /// Conditionally projects elements of the stream to another stream
        /// </summary>
        public static IReactiveStream<T> Where<T>(this IObservable<T> stream, Func<T, bool> predicate)
        {
            return new WhereConditionalStream<T>(stream, predicate);
        }
    }
}
