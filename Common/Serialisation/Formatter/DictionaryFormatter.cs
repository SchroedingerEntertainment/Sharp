// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides base functionality for the IDictionary serialization formatter
    /// </summary>
    public static class DictionaryFormatter
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
        public static void Write(Stream serializationStream, IDictionary value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Dictionary);
            }
            serializationStream.EncodeVariableInt((UInt32)value.Count);
            Type[] generic = ArrayExtension.Empty<Type>();
            Type type = value.GetType();
            do
            {
                generic = type.GetGenericArguments();
                if (type.BaseType != null)
                {
                    type = type.BaseType;
                }
                else throw new SerializationException(value.GetType().FullName);
            }
            while (generic.Length != 2);
            TypeCodes globalKeyCode = TypeFormatter.GetTypeCodes(generic[0]);
            TypeCodes globalValueCode = TypeFormatter.GetTypeCodes(generic[1]);
            bool isExplicitKeyType = (globalKeyCode != TypeCodes.Object);
            bool isExplicitValueType = (globalValueCode != TypeCodes.Object);

            serializationStream.Put((byte)globalKeyCode);
            serializationStream.Put((byte)globalValueCode);
            foreach (DictionaryEntry item in value)
            {
                if (isExplicitKeyType)
                {
                    TypeFormatter.Serialize(serializationStream, 0, item.Key, false);
                }
                else TypeFormatter.Serialize(serializationStream, item.Key);
                if (isExplicitValueType)
                {
                    TypeFormatter.Serialize(serializationStream, 0, item.Value, false);
                }
                else TypeFormatter.Serialize(serializationStream, item.Value);
            }
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <param name="fieldType">The underlaying field type to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static IDictionary Read(Stream serializationStream, Type fieldType)
        {
            int count = (int)serializationStream.ToVariableInt();

            IDictionary value = fieldType.CreateInstance<IDictionary>();
            Type[] generic = fieldType.GetGenericArguments();
            TypeCodes globalKeyCode = (TypeCodes)serializationStream.Get();
            TypeCodes globalValueCode = (TypeCodes)serializationStream.Get();
            bool isExplicitKeyType = (globalKeyCode != TypeCodes.Object);
            bool isExplicitValueType = (globalValueCode != TypeCodes.Object);

            for (; count > 0; count--)
            {
                object key; if (isExplicitKeyType)
                {
                    key = TypeFormatter.Deserialize(serializationStream, (UInt32)globalKeyCode, generic[0]);
                }
                else key = TypeFormatter.Deserialize(serializationStream);
                object val; if (isExplicitValueType)
                {
                    val = TypeFormatter.Deserialize(serializationStream, (UInt32)globalValueCode, generic[1]);
                }
                else val = TypeFormatter.Deserialize(serializationStream);
                value.Add(key, val);
            }
            return value;
        }
    }
}
