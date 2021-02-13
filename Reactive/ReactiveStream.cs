// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE.Reactive
{
    /// <summary>
    /// A generic reactive data stream
    /// </summary>
    public abstract class ReactiveStream<T> : IReactiveStream<T>, IDisposable
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct Disposer : IDisposable
        {
            IObserver<T> observer;

            ReactiveStream<T> owner;
            /// <summary>
            /// The stream instance this subscription is bound to
            /// </summary>
            public ReactiveStream<T> Owner
            {
                get { return owner; }
            }

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public Disposer(ReactiveStream<T> owner, IObserver<T> observer)
            {
                this.owner = owner;
                this.observer = observer;
            }
            public void Dispose()
            {
                owner.subscriptionLock.WriteLock();
                try
                {
                    owner.subscriptions.Remove(observer);
                }
                finally
                {
                    owner.subscriptionLock.WriteRelease();
                }
            }
        }

        protected HashSet<IObserver<T>> subscriptions;
        protected ReadWriteLock subscriptionLock;

        /// <summary>
        /// Creates a new generic reactive data stream instance
        /// </summary>
        public ReactiveStream()
        {
            this.subscriptions = new HashSet<IObserver<T>>();
            this.subscriptionLock = new ReadWriteLock();
        }
        public void Dispose()
        {
            if (subscriptions.Count > 0)
            {
                subscriptionLock.WriteLock();
                try
                {
                    foreach (IObserver<T> observer in subscriptions)
                    {
                        try
                        {
                            observer.OnCompleted();
                        }
                        catch (Exception er)
                        {
                            try
                            {
                                observer.OnError(er);
                            }
                            catch (Exception internalError)
                            {
                                OnUnhandledException(observer, internalError);
                            }
                        }
                    }
                    subscriptions.Clear();
                }
                finally
                {
                    subscriptionLock.WriteRelease();
                }
            }
        }

        /// <summary>
        /// Pushes a new value to the stream
        /// </summary>
        protected virtual void Push(T value)
        {
            subscriptionLock.ReadLock();
            try
            {
                foreach (IObserver<T> observer in subscriptions)
                    try
                    {
                        observer.OnNext(value);
                    }
                    catch (Exception er)
                    {
                        try
                        {
                            observer.OnError(er);
                        }
                        catch (Exception internalError)
                        {
                            OnUnhandledException(observer, internalError);
                        }
                    }
            }
            finally
            {
                subscriptionLock.ReadRelease();
            }
        }
        /// <summary>
        /// Pushes a new exception to the stream
        /// </summary>
        protected virtual void Push(Exception error)
        {
            subscriptionLock.ReadLock();
            try
            {
                foreach (IObserver<T> observer in subscriptions)
                    try
                    {
                        observer.OnError(error);
                    }
                    catch (Exception er) 
                    {
                        OnUnhandledException(observer, er);
                    }
            }
            finally
            {
                subscriptionLock.ReadRelease();
            }
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            subscriptionLock.WriteLock();
            try
            {
                subscriptions.Add(observer);
            }
            finally
            {
                subscriptionLock.WriteRelease();
            }
            return new Disposer(this, observer);
        }

        /// <summary>
        /// Callback to handle exceptions not able to be processed by an observer
        /// </summary>
        /// <param name="observer">The observer instance that caused the exception</param>
        /// <param name="error">The exception thrown by the observer instance</param>
        protected virtual void OnUnhandledException(IObserver<T> observer, Exception error)
        { }

        /// <summary>
        /// Wraps a subscription action into a push-based notification stream
        /// </summary>
        public static IReactiveStream<T> Create(Func<IObserver<T>, IDisposable> action)
        {
            return new AnonymousStream<T>(action);
        }
    }
}
