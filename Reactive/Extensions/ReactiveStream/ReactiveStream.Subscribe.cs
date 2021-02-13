// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class ReactiveStreamExtension
    {
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T, Result>(this IReactiveStream<T, Result> stream, IReceiver<T, Result> receiver)
        {
            return stream.Subscribe(receiver);
        }

        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T, Result>(this IReactiveStream<T, Result> stream, Func<T, Result> next, Func<Exception, Result> error, Action completed)
        {
            return stream.Subscribe(new AnonymousReceiver<T, Result>(next, error, completed));
        }
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T, Result>(this IReactiveStream<T, Result> stream, Func<T, Result> next, Func<Exception, Result> error)
        {
            return Subscribe<T, Result>(stream, next, error, null);
        }
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T, Result>(this IReactiveStream<T, Result> stream, Func<T, Result> next, Action completed)
        {
            return Subscribe<T, Result>(stream, next, null, completed);
        }
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T, Result>(this IReactiveStream<T, Result> stream, Func<T, Result> action)
        {
            return Subscribe<T, Result>(stream, action, null, null);
        }
    }
}
