// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SE.Reactive
{
    public static partial class ObservableExtension
    {
        /// <summary>
        /// Subscribes a Task object to this stream
        /// </summary>
        public static Task<T> GetTask<T>(this IObservable<T> stream)
        {
            return new TaskCompletionReceiver<T>(stream).Task;
        }
    }
}