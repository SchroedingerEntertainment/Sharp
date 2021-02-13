// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a Task into a push-based notification stream
    /// </summary>
    public class TaskStream : IReactiveStream<bool>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct Disposer : IDisposable
        {
            IObserver<bool> observer;

            TaskStream owner;
            /// <summary>
            /// The stream instance this subscription is bound to
            /// </summary>
            public TaskStream Owner
            {
                get { return owner; }
            }

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public Disposer(TaskStream owner, IObserver<bool> observer)
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

        Task task;
        HashSet<IObserver<bool>> subscriptions;
        ReadWriteLock subscriptionLock;
        bool signal;

        /// <summary>
        /// Embedds the passed task into a new stream instance
        /// </summary>
        public TaskStream(Task task)
        {
            this.task = task;
            this.subscriptions = new HashSet<IObserver<bool>>();
            this.subscriptionLock = new ReadWriteLock();
            this.task.ContinueWith(Callback);
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {

        Head:
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    {
                        try
                        {
                            observer.OnNext(false);
                            observer.OnCompleted();
                        }
                        catch (Exception er)
                        {
                            try
                            {
                                observer.OnError(er);
                            }
                            catch { }
                        }
                    }
                    return VoidDisposer.Instance;
                case TaskStatus.Faulted:
                    {
                        observer.OnError(task.Exception.InnerException);
                    }
                    return VoidDisposer.Instance;
                case TaskStatus.Canceled:
                    {
                        observer.OnError(new TaskCanceledException(task));
                    }
                    return VoidDisposer.Instance;
                default:
                    {
                        subscriptionLock.WriteLock();
                        try
                        {
                            if (!signal)
                            {
                                subscriptions.Add(observer);
                            }
                            else goto Head;
                        }
                        finally
                        {
                            subscriptionLock.WriteRelease();
                        }
                    }
                    return new Disposer(this, observer);
            }
        }

        void Callback(Task task)
        {
            subscriptionLock.WriteLock();
            try
            {
                signal = true;
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            foreach (IObserver<bool> observer in subscriptions)
                                try
                                {
                                    observer.OnNext(false);
                                    observer.OnCompleted();
                                }
                                catch (Exception er)
                                {
                                    try
                                    {
                                        observer.OnError(er);
                                    }
                                    catch { }
                                }
                        }
                        break;
                    case TaskStatus.Faulted:
                        {
                            foreach (IObserver<bool> observer in subscriptions)
                                observer.OnError(task.Exception.InnerException);
                        }
                        break;
                    case TaskStatus.Canceled:
                        {
                            foreach (IObserver<bool> observer in subscriptions)
                                observer.OnError(new TaskCanceledException(task));
                        }
                        break;
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
    /// Wraps a Task into a push-based notification stream
    /// </summary>
    public class TaskStream<T> : IReactiveStream<T>
    {
        /// <summary>
        /// Manages a subscription bound to the owning streamn
        /// </summary>
        public struct Disposer : IDisposable
        {
            IObserver<T> observer;

            TaskStream<T> owner;
            /// <summary>
            /// The stream instance this subscription is bound to
            /// </summary>
            public TaskStream<T> Owner
            {
                get { return owner; }
            }

            /// <summary>
            /// Creates a new subscription to a stream
            /// </summary>
            /// <remarks>To be used by the stream only</remarks>
            public Disposer(TaskStream<T> owner, IObserver<T> observer)
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

        Task<T> task;
        HashSet<IObserver<T>> subscriptions;
        ReadWriteLock subscriptionLock;
        bool signal;

        /// <summary>
        /// Embedds the passed task into a new stream instance
        /// </summary>
        public TaskStream(Task<T> task)
        {
            this.task = task;
            this.subscriptions = new HashSet<IObserver<T>>();
            this.subscriptionLock = new ReadWriteLock();
            this.task.ContinueWith(Callback);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {

        Head:
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    {
                        try
                        {
                            observer.OnNext(task.Result);
                            observer.OnCompleted();
                        }
                        catch (Exception er)
                        {
                            try
                            {
                                observer.OnError(er);
                            }
                            catch { }
                        }
                    }
                    return VoidDisposer.Instance;
                case TaskStatus.Faulted:
                    {
                        observer.OnError(task.Exception.InnerException);
                    }
                    return VoidDisposer.Instance;
                case TaskStatus.Canceled:
                    {
                        observer.OnError(new TaskCanceledException(task));
                    }
                    return VoidDisposer.Instance;
                default:
                    {
                        subscriptionLock.WriteLock();
                        try
                        {
                            if (!signal)
                            {
                                subscriptions.Add(observer);
                            }
                            else goto Head;
                        }
                        finally
                        {
                            subscriptionLock.WriteRelease();
                        }
                    }
                    return new Disposer(this, observer);
            }
        }

        void Callback(Task task)
        {
            subscriptionLock.WriteLock();
            try
            {
                signal = true;
                switch (task.Status)
                {
                    case TaskStatus.RanToCompletion:
                        {
                            Task<T> tmp = task as Task<T>;
                            foreach (IObserver<T> observer in subscriptions)
                                try
                                {
                                    observer.OnNext(tmp.Result);
                                    observer.OnCompleted();
                                }
                                catch (Exception er)
                                {
                                    try
                                    {
                                        observer.OnError(er);
                                    }
                                    catch { }
                                }
                        }
                        break;
                    case TaskStatus.Faulted:
                        {
                            foreach (IObserver<T> observer in subscriptions)
                                observer.OnError(task.Exception.InnerException);
                        }
                        break;
                    case TaskStatus.Canceled:
                        {
                            foreach (IObserver<T> observer in subscriptions)
                                observer.OnError(new TaskCanceledException(task));
                        }
                        break;
                }
                subscriptions.Clear();
            }
            finally
            {
                subscriptionLock.WriteRelease();
            }
        }
    }
}
