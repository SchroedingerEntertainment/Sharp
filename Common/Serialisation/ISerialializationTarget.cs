// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Defines callbacks to be used on a serialization process
    /// </summary>
    public interface ISerialializationTarget
    {
        /// <summary>
        /// A callback executed before values are read
        /// </summary>
        void OnBeforeSerialization();
        /// <summary>
        /// A callback executed after values have been read
        /// </summary>
        void OnAfterSerialization();
    }
}
