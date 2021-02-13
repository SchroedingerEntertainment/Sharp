// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Forwards from one stream into another continous data stream
    /// </summary>
    public struct PredicateStream<T, Result> : IReactiveStream<T>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct PredicateConnector : IReceiver<T, Result>
        {
            readonly Func<T, Result> predicate;
            readonly IObserver<T> receiver;

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public PredicateConnector(IObserver<T> receiver, Func<T, Result> predicate)
            {
                this.predicate = predicate;
                this.receiver = receiver;
            }

            public Result OnNext(T value)
            {
                try
                {
                    receiver.OnNext(value);
                    return predicate(value);
                }
                catch (Exception er)
                {
                    receiver.OnError(er);
                }
                return default(Result);
            }
            public Result OnError(Exception error)
            {
                receiver.OnError(error);
                return default(Result);
            }
            public void OnCompleted()
            {
                receiver.OnCompleted();
            }
        }

        readonly IReactiveStream<T, Result> stream;
        readonly Func<T, Result> predicate;

        /// <summary>
        /// Creates a wrapper instance around the passed predicate
        /// </summary>
        public PredicateStream(IReactiveStream<T, Result> stream, Func<T, Result> predicate)
        {
            this.predicate = predicate;
            this.stream = stream;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return stream.Subscribe(new PredicateConnector(observer, predicate));
        }
    }
}