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
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Identities the state machine type for this method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [Serializable]
    public class StateMachineAttribute : Attribute
    {
        /// <summary>
        /// Gets the type that implements the state machine.
        /// </summary>
        public Type StateMachineType { get; private set; }

        /// <summary>
        /// Initializes the attribute.
        /// </summary>
        /// <param name="stateMachineType">The type that implements the state machine.</param>
        public StateMachineAttribute(Type stateMachineType)
        {
            StateMachineType = stateMachineType;
        }
    }
}
#endif