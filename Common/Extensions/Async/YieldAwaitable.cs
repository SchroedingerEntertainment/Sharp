﻿// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// AsyncMethodBuilder.cs
//
// <OWNER>Microsoft</OWNER>
//
// Compiler-targeted types for use of asynchronous methods.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

#if net40
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    ///   Provides an awaitable context for switching into a target environment.
    /// </summary>
    /// <remarks>
    ///   This type is intended for compiler use only.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct YieldAwaitable
    {
        /// <summary>
        ///   Gets an awaiter for this <see cref="YieldAwaitable" /> .
        /// </summary>
        /// <returns> An awaiter for this awaitable. </returns>
        /// <remarks>
        ///   This method is intended for compiler user rather than use directly in code.
        /// </remarks>
        public YieldAwaiter GetAwaiter()
        {
            return new YieldAwaiter();
        }

        /// <summary>
        ///   Provides an awaiter that switches into a target environment.
        /// </summary>
        /// <remarks>
        ///   This type is intended for compiler use only.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct YieldAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>
            ///   Gets whether a yield is not required.
            /// </summary>
            /// <remarks>
            ///   This property is intended for compiler user rather than use directly in code.
            /// </remarks>
            public bool IsCompleted
            {
                get { return false; }
            }

            /// <summary>
            ///   Posts the <paramref name="continuation" /> back to the current context.
            /// </summary>
            /// <param name="continuation"> The action to invoke asynchronously. </param>
            /// <exception cref="InvalidOperationException">The awaiter was not properly initialized.</exception>
            public void OnCompleted(Action continuation)
            {
                Taskʾ.CompletedTask.GetAwaiter().OnCompleted(continuation);
            }

            /// <summary>
            ///   Posts the <paramref name="continuation" /> back to the current context.
            /// </summary>
            /// <param name="continuation"> The action to invoke asynchronously. </param>
            /// <exception cref="InvalidOperationException">The awaiter was not properly initialized.</exception>
            [SecurityCritical]
            public void UnsafeOnCompleted(Action continuation)
            {
                Taskʾ.CompletedTask.GetAwaiter().UnsafeOnCompleted(continuation);
            }

            /// <summary>
            ///   Ends the await operation.
            /// </summary>
            public void GetResult()
            { }
        }
    }
}
#endif