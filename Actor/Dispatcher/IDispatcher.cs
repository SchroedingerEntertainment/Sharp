// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Reactive;

namespace SE.Actor
{
    /// <summary>
    /// The basic interface of channel dispatching behavior
    /// </summary>
    public interface IDispatcher<TMessage> : IDisposable
    {
        /// <summary>
        /// Get the number of subscribers registered
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Registers a new subscriber based on the channel
        /// </summary>
        /// <param name="observer">Defines the execution behavior</param>
        void Register(IReceiver<TMessage, bool> observer);

        /// <summary>
        /// Removes the subscriber from the list of dispatching targets
        /// </summary>
        /// <param name="adapter">The subscriber to remove</param>
        void Remove(IReceiver<TMessage, bool> observer);

        /// <summary>
        /// Dispatches the payload based on the implementation dispatching strategy
        /// to registered subscribers
        /// </summary>
        /// <param name="message">The message to share</param>
        bool Dispatch(ref TMessage message);

        /// <summary>
        /// Removes any subscriber from the list of dispatching targets
        /// </summary>
        void Clear();
    }
}
