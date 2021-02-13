// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Reactive;

namespace SE.Actor
{
    public class MessageStream<TMessage> : IReactiveStream<TMessage, bool>, IDisposable
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public class Disposer : FinalizerObject
        {
            IReceiver<TMessage, bool> observer;

            MessageStream<TMessage> owner;
            /// <summary>
            /// The stream instance this subscription is bound to
            /// </summary>
            public MessageStream<TMessage> Owner
            {
                get { return owner; }
            }

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public Disposer(MessageStream<TMessage> owner, IReceiver<TMessage, bool> observer)
            {
                this.owner = owner;
                this.observer = observer;
            }
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    owner.dispatcher.Remove(observer);
                }
                base.Dispose(disposing);
            }
        }

        MessageStreamContainer<TMessage> nestedStreams;
        /// <summary>
        /// Contains conditionally nested streams to this one
        /// </summary>
        public MessageStreamContainer<TMessage> NestedStreams
        {
            get { return nestedStreams; }
        }

        IDispatcher<TMessage> dispatcher;

        /// <summary>
        /// Creates a new reactive message stream instance
        /// </summary>
        /// <param name="dispatcher">Provides the dispatching startegy used by this stream</param>
        public MessageStream(IDispatcher<TMessage> dispatcher)
        {
            this.dispatcher = dispatcher;
            this.nestedStreams = new MessageStreamContainer<TMessage>();
        }
        /// <summary>
        /// Creates a new reactive message stream instance
        /// </summary>
        /// <param name="dispatcher">Provides the dispatching startegy used by this stream</param>
        /// <param name="nestedContainer">Provides handling of nested stream requests</param>
        public MessageStream(IDispatcher<TMessage> dispatcher, MessageStreamContainer<TMessage> nestedContainer)
        {
            this.dispatcher = dispatcher;
            this.nestedStreams = nestedContainer;
        }
        public void Dispose()
        {
            dispatcher.Dispose();
        }

        /// <summary>
        /// Dispatches the provided message to the stream
        /// </summary>
        public bool Dispatch(ref TMessage message)
        {
            return dispatcher.Dispatch(ref message);
        }
        /// <summary>
        /// Dispatches the provided message to the stream
        /// </summary>
        public bool Dispatch(TMessage message)
        {
            return dispatcher.Dispatch(ref message);
        }

        public IDisposable Subscribe(IReceiver<TMessage, bool> observer)
        {
            dispatcher.Register(observer);
            return new Disposer(this, observer);
        }
    }
}
