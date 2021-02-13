// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System
{
    public static partial class ArrayExtension
    {
        #if DEBUG
        private readonly static Type StateMashineAttribute = typeof(Runtime.CompilerServices.AsyncStateMachineAttribute);
        #endif

        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>An awaitable to the asynchronous vectoring operation</returns>
        public static Task ParallelFor<T>(this T[] items, Action<T> action)
        {
            #if DEBUG
            System.Diagnostics.Debug.Assert (!action.Method.IsDefined(StateMashineAttribute, false));
            #endif
            return Taskʾ.Run(() => Parallel.ForEach(items, action));
        }

        /// <summary>
        /// Executes an action parallel for each item in the data vector
        /// </summary>
        /// <param name="action">An action to execute</param>
        /// <returns>An awaitable to the asynchronous vectoring operation</returns>
        public static void ForEach<T>(this T[] items, Action<T> action)
        {
            #if DEBUG
            System.Diagnostics.Debug.Assert (!action.Method.IsDefined(StateMashineAttribute, false));
            #endif
            Parallel.ForEach(items, action);
        }
    }
}
