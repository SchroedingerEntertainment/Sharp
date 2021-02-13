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
        /// Configures an awaiter used to await this Task.
        /// </summary>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original context captured; otherwise, false.
        /// </param>
        /// <returns>An object used to await this task.</returns>
        public static ConfiguredTaskAwaitable ConfigureAwait(this Task task, bool continueOnCapturedContext)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
        }
        /// <summary>
        /// Configures an awaiter used to await this Task.
        /// </summary>
        /// <param name="continueOnCapturedContext">
        /// true to attempt to marshal the continuation back to the original context captured; otherwise, false.
        /// </param>
        /// <returns>An object used to await this task.</returns>
        public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task, bool continueOnCapturedContext)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }
            return new ConfiguredTaskAwaitable<TResult>(task, continueOnCapturedContext);
        }
    }
}
#endif