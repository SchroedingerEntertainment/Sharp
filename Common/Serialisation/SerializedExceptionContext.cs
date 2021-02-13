// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Contains information from a deserialized exception
    /// </summary>
    public class SerializedExceptionContext : Exception
    {
        string stackTrace;
        public override string StackTrace
        {
            get { return stackTrace; }
        }

        Type exceptionType;
        /// <summary>
        /// Gets or sets the type of the original exception thrown 
        /// </summary>
        public Type ExceptionType
        {
            get { return exceptionType; }
            set { exceptionType = value; }
        }

        /// <summary>
        /// Creates a new instance with given message and stack trace
        /// </summary>
        public SerializedExceptionContext(string message, string stackTrace)
            : base(message)
        {
            this.stackTrace = stackTrace;
        }
    }
}
