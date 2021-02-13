// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a stream instance into an awaitable completion source
    /// </summary>
    public struct TaskCompletionReceiver<T> : IReceiver<T>
    {
        IDisposable subscription;

        TaskCompletionSource<T> awaitable;
        /// <summary>
        /// Obtains the Task object assigned to the stream
        /// </summary>
        public Task<T> Task
        {
            get { return awaitable.Task; }
        }

        /// <summary>
        /// Creates a wrapper around the stream instance
        /// </summary>
        public TaskCompletionReceiver(IObservable<T> stream)
        {
            this.awaitable = new TaskCompletionSource<T>();
            this.subscription = null;
            this.subscription = stream.Subscribe(this);
        }

        public void OnNext(T value)
        {
            awaitable.TrySetResult(value);
            subscription.Dispose();
        }
        public void OnError(Exception error)
        {
            awaitable.TrySetException(error);
            subscription.Dispose();
        }
        public void OnCompleted()
        {
            awaitable.TrySetCanceled();
        }
    }
}
