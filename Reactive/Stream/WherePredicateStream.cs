// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps conditional forwarding into an continous data stream
    /// </summary>
    public struct WherePredicateStream<T> : IReactiveStream<T>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct WherePredicateConnector : IReceiver<T, bool>
        {
            readonly Func<T, bool> predicate;
            readonly IObserver<T> receiver;

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public WherePredicateConnector(IObserver<T> receiver, Func<T, bool> predicate)
            {
                this.predicate = predicate;
                this.receiver = receiver;
            }

            public bool OnNext(T value)
            {
                try
                {
                    if (predicate(value))
                    {
                        receiver.OnNext(value);
                        return true;
                    }
                }
                catch (Exception er)
                {
                    receiver.OnError(er);
                }
                return false;
            }
            public bool OnError(Exception error)
            {
                receiver.OnError(error);
                return false;
            }
            public void OnCompleted()
            {
                receiver.OnCompleted();
            }
        }

        readonly IReactiveStream<T, bool> stream;
        readonly Func<T, bool> predicate;

        /// <summary>
        /// Creates a wrapper instance around the passed conditional predicate
        /// </summary>
        public WherePredicateStream(IReactiveStream<T, bool> stream, Func<T, bool> predicate)
        {
            this.predicate = predicate;
            this.stream = stream;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return stream.Subscribe(new WherePredicateConnector(observer, predicate));
        }
    }
}