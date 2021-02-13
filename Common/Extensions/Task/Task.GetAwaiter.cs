// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

#if net40
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static partial class TaskExtension
    {
        /// <summary>
        /// Gets an awaiter used to await this Task.
        /// </summary>
        /// <remarks>
        /// This method is intended for compiler use rather than for use in application code.
        /// </remarks>
        /// <returns>An awaiter instance.</returns>
        public static TaskAwaiter GetAwaiter(this Task task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            else return new TaskAwaiter(task);
        }
        /// <summary>
        /// Gets an awaiter used to await this Task.
        /// </summary>
        /// <remarks>
        /// This method is intended for compiler use rather than for use in application code.
        /// </remarks>
        /// <returns>An awaiter instance.</returns>
        public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            else return new TaskAwaiter<TResult>(task);
        }
    }
}
#endif