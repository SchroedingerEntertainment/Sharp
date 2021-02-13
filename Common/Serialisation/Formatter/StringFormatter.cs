// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides base functionality for the string serialization formatter
    /// </summary>
    public static class StringFormatter
    {
        /// <summary>
        /// Serializes a data object to the provided stream
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a 
        /// variety of backing stores (such as files, network, memory, and so on)
        /// </param>
        /// <param name="value">
        /// The object, or root of the object graph, to serialize. All child objects of this 
        /// root object are automatically serialized
        /// </param>
        /// <param name="addTypeCode">Defines if a type code flag should be set in front of the serialized data</param>
        public static void Write(Stream serializationStream, string value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.String);
            }
            serializationStream.EncodeVariableInt((UInt32)value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                serializationStream.Encode(value[i]);
            }
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static string Read(Stream serializationStream)
        {
            StringBuilder buffer = new StringBuilder((int)serializationStream.ToVariableInt());
            for (int i = 0; i < buffer.Capacity; i++)
            {
                buffer.Append((char)serializationStream.ToInt16());
            }
            return buffer.ToString();
        }
    }
}
