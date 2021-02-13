// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps conditional forwarding into an continous data stream
    /// </summary>
    public struct WhereConditionalStream<T> : IReactiveStream<T>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct WhereConditionalConnector : IReceiver<T>
        {
            readonly Func<T, bool> predicate;
            readonly IObserver<T> receiver;

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public WhereConditionalConnector(IObserver<T> receiver, Func<T, bool> predicate)
            {
                this.predicate = predicate;
                this.receiver = receiver;
            }

            public void OnNext(T value)
            {
                try
                {
                    if (predicate(value))
                        receiver.OnNext(value);
                }
                catch (Exception er)
                {
                    receiver.OnError(er);
                }
            }
            public void OnError(Exception error)
            {
                receiver.OnError(error);
            }
            public void OnCompleted()
            {
                receiver.OnCompleted();
            }
        }

        readonly IObservable<T> stream;
        readonly Func<T, bool> predicate;

        /// <summary>
        /// Creates a wrapper instance around the passed conditional predicate
        /// </summary>
        public WhereConditionalStream(IObservable<T> stream, Func<T, bool> predicate)
        {
            this.predicate = predicate;
            this.stream = stream;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return stream.Subscribe(new WhereConditionalConnector(observer, predicate));
        }
    }
}