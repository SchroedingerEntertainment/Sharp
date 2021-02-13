// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Identifies the index in a serialization data stream
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SerializedAttribute : Attribute
    {
        int index;
        /// <summary>
        /// A zero based integer index pointing to the property in a
        /// serialized data stream
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// Identifies this field as being serialized from the TypeFormatter if requested
        /// </summary>
        /// <param name="index">A zero based integer index pointing to the property in a
        /// serialized data stream</param>
        public SerializedAttribute(int index)
        {
            this.index = index;
        }
    }
}
