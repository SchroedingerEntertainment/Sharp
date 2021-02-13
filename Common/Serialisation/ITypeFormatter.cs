// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// An interface to a specialized type serializer
    /// </summary>
    public interface ITypeFormatter
    {
        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        object Deserialize(Stream serializationStream);

        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a 
        /// variety of backing stores (such as files, network, memory, and so on)
        /// </param>
        /// <param name="graph">
        /// The object, or root of the object graph, to serialize. All child objects of this 
        /// root object are automatically serialized
        /// </param>
        void Serialize(Stream serializationStream, object graph);
    }
}
