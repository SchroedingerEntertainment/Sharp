// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides base functionality for the PathDescriptor serialization formatter
    /// </summary>
    public class PathFormatter : ITypeFormatter
    {
        public object Deserialize(Stream serializationStream)
        {
            string path = TypeFormatter.Deserialize(serializationStream, (UInt32)TypeCodes.String) as string;
            return new PathDescriptor(path);
        }
        public void Serialize(Stream serializationStream, object graph)
        {
            TypeFormatter.Serialize(serializationStream, (UInt32)TypeCodes.String, (graph as PathDescriptor).GetAbsolutePath(), false);
        }
    }
}
