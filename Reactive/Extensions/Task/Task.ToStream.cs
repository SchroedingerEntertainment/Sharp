// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SE.Reactive
{
    public static partial class TaskExtension
    {
        /// <summary>
        /// Converts this task into a push-based notification stream
        /// </summary>
        public static TaskStream ToStream(this Task task)
        {
            return new TaskStream(task);
        }
        /// <summary>
        /// Converts this task into a push-based notification stream
        /// </summary>
        public static TaskStream<T> ToStream<T>(this Task<T> task)
        {
            return new TaskStream<T>(task);
        }
    }
}
