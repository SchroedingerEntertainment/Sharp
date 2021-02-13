﻿// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class EnumerableExtension
    {
        /// <summary>
        /// Converts the collection of items into a push-based notification stream
        /// </summary>
        public static IReactiveStream<T> ToStream<T>(this IEnumerable<T> items)
        {
            return new EnumerableStream<T>(items);
        }
    }
}
