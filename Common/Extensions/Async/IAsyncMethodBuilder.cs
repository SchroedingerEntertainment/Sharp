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
    /// Represents an asynchronous method builder.
    /// </summary>
    internal interface IAsyncMethodBuilder
    {
        void PreBoxInitialization();
    }
}
#endif