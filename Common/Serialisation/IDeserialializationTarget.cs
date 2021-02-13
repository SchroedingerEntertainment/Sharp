// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Defines callbacks to be used on a deserialization process
    /// </summary>
    public interface IDeserialializationTarget
    {
        /// <summary>
        /// A callback executed after the instance has been created
        /// </summary>
        void OnBeforeDeserialization();
        /// <summary>
        /// A callback executed after values have been set
        /// </summary>
        void OnAfterDeserialization();
    }
}
