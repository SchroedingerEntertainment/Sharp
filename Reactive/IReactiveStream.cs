// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Defines a provider for push-based notification
    /// </summary>
    public interface IReactiveStream<T> : IObservable<T>
    { }

    /// <summary>
    /// Defines a provider for push-based notification
    /// </summary>
    public interface IReactiveStream<T, Result>
    {
        /// <summary>
        /// Notifies the provider that an observer is to receive notifications
        /// </summary>
        /// <param name="observer">The object that is to receive notifications</param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving 
        /// notifications before the provider has finished sending them
        /// </returns>
        IDisposable Subscribe(IReceiver<T, Result> observer);
    }
}
