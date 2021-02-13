// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a type-change into an continous data stream
    /// </summary>
    public struct WhereSelectStream<TSource, TResult> : IReactiveStream<TResult>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct WhereSelectConnector : IReceiver<TSource>
        {
            readonly Func<TSource, TResult> selector;
            readonly IObserver<TResult> receiver;

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public WhereSelectConnector(IObserver<TResult> receiver, Func<TSource, TResult> selector)
            {
                this.selector = selector;
                this.receiver = receiver;
            }

            public void OnNext(TSource value)
            {
                try
                {
                    receiver.OnNext(selector(value));
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

        readonly IObservable<TSource> stream;
        readonly Func<TSource, TResult> selector;

        /// <summary>
        /// Creates a wrapper instance around the passed selector
        /// </summary>
        public WhereSelectStream(IObservable<TSource> stream, Func<TSource, TResult> selector)
        {
            this.stream = stream;
            this.selector = selector;
        }

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            return stream.Subscribe(new WhereSelectConnector(observer, selector));
        }
    }
    /// <summary>
    /// Wraps a type-change into an continous data stream
    /// </summary>
    public struct WhereSelectStream<InSource, Result, OutSource> : IReactiveStream<OutSource, Result>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct WhereSelectConnector : IReceiver<InSource, Result>
        {
            readonly Func<InSource, OutSource> selector;
            readonly IReceiver<OutSource, Result> receiver;

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public WhereSelectConnector(IReceiver<OutSource, Result> receiver, Func<InSource, OutSource> selector)
            {
                this.selector = selector;
                this.receiver = receiver;
            }

            public Result OnNext(InSource value)
            {
                try
                {
                    return receiver.OnNext(selector(value));
                }
                catch (Exception er)
                {
                    return receiver.OnError(er);
                }
            }
            public Result OnError(Exception error)
            {
                return receiver.OnError(error);
            }
            public void OnCompleted()
            {
                receiver.OnCompleted();
            }
        }

        readonly IReactiveStream<InSource, Result> stream;
        readonly Func<InSource, OutSource> selector;

        /// <summary>
        /// Creates a wrapper instance around the passed selector
        /// </summary>
        public WhereSelectStream(IReactiveStream<InSource, Result> stream, Func<InSource, OutSource> selector)
        {
            this.stream = stream;
            this.selector = selector;
        }

        public IDisposable Subscribe(IReceiver<OutSource, Result> observer)
        {
            return stream.Subscribe(new WhereSelectConnector(observer, selector));
        }
    }
}