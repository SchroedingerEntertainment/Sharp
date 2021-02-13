// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Reactive;

namespace SE.Actor
{
    public static partial class MessageStreamExtension
    {
        /// <summary>
        /// Conditionally projects elements of the stream to another stream
        /// </summary>
        public static IReactiveStream<T, bool> Where<T>(this MessageStream<T> stream, params object[] parameter)
        {
            IReactiveStream<T, bool> result; if (!stream.NestedStreams.TryGet(parameter, out result))
            {
                result = stream;
            }
            return result;
        }
    }
}
