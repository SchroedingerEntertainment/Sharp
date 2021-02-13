// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    public static partial class ObservableExtension
    {
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> stream, IObserver<T> receiver)
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
        public static IDisposable Subscribe<T>(this IObservable<T> stream, Action<T> next, Action<Exception> error, Action completed)
        {
            return stream.Subscribe(new AnonymousReceiver<T>(next, error, completed));
        }
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> stream, Action<T> next, Action<Exception> error)
        {
            return Subscribe<T>(stream, next, error, null);
        }
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> stream, Action<T> next, Action completed)
        {
            return Subscribe<T>(stream, next, null, completed);
        }
        /// <summary>
        /// Notifies the stream that a receiver is ready to receive notifications
        /// </summary>
        /// <returns>
        /// A reference to an interface that allows receivers to stop receiving notifications
        /// before the stream has finished sending them
        ///</returns>
        public static IDisposable Subscribe<T>(this IObservable<T> stream, Action<T> action)
        {
            return Subscribe<T>(stream, action, null, null);
        }
    }
}
