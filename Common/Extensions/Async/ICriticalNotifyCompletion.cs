// ==++==
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
using System.Security;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Represents an awaiter used to schedule continuations when an await operation completes.
    /// </summary>
    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        /// <summary>
        /// Schedules the continuation action to be invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        /// <remarks>
        /// Unlike OnCompleted, UnsafeOnCompleted need not propagate ExecutionContext information.
        /// </remarks>
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }
}
#endif