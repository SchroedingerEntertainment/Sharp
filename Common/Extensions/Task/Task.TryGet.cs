// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static partial class TaskExtension
    {
        /// <summary>
        /// Tries to obtain the result from this task without throwing an exception
        /// </summary>
        /// <param name="result">The result set on success</param>
        /// <returns>True if the task succeeded, false otherwise</returns>
        public static bool TryGetResult<T>(this Task<T> task, out T result)
        {
            switch (task.Status)
            {
                case TaskStatus.Faulted:
                case TaskStatus.Canceled:
                    {
                        result = default(T);
                    }
                    return false;
                case TaskStatus.RanToCompletion:
                    {
                        result = task.Result;
                    }
                    return true;
                default: if (!Join((Task)task))
                    {
                        goto case TaskStatus.RanToCompletion;
                    }
                    else goto case TaskStatus.Faulted;
            }
        }
        /// <summary>
        /// Tries to obtain the result from this task without throwing an exception
        /// </summary>
        /// <returns>A helper struct that may contain a value, based on the outcome of the task</returns>
        public async static Task<Any<T>> TryGetResult<T>(this Task<T> task)
        {
            try
            {
                await task;
            }
            catch { }
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: return new Any<T>(task.Result);
                default: return Any<T>.Empty;
            }
        }
    }
}