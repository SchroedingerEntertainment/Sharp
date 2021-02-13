﻿// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Reactive
{
    /// <summary>
    /// Wraps a set of actions for receiving push-based notifications
    /// </summary>
    public struct AnonymousReceiver<T> : IReceiver<T>
    {
        Action<T> onNext;
        Action<Exception> onError;
        Action onCompleted;

        /// <summary>
        /// Creates a wrapper instance around the passed set of actions
        /// </summary>
        public AnonymousReceiver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
            if (onNext != null)
                onNext(value);
        }
        public void OnError(Exception error)
        {
            if (onError != null)
                onError(error);
        }
        public void OnCompleted()
        {
            if (onCompleted != null)
                onCompleted();
        }
    }
    /// <summary>
    /// Wraps a set of actions for receiving push-based notifications
    /// </summary>
    public struct AnonymousReceiver<T, Result> : IReceiver<T, Result>
    {
        Func<T, Result> onNext;
        Func<Exception, Result> onError;
        Action onCompleted;

        /// <summary>
        /// Creates a wrapper instance around the passed set of actions
        /// </summary>
        public AnonymousReceiver(Func<T, Result> onNext, Func<Exception, Result> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public Result OnNext(T value)
        {
            if (onNext != null)
            {
                return onNext(value);
            }
            else return default(Result);
        }
        public Result OnError(Exception error)
        {
            if (onError != null)
            {
                return onError(error);
            }
            else return default(Result);
        }
        public void OnCompleted()
        {
            if (onCompleted != null)
                onCompleted();
        }
    }
}
