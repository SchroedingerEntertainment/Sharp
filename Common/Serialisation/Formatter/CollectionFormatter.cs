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
    /// Provides base functionality for the ICollection serialization formatter
    /// </summary>
    public static class CollectionFormatter
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
        public static void Write(Stream serializationStream, ICollection value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Collection);
            }
            serializationStream.EncodeVariableInt((UInt32)value.Count);
            Type collection = value.GetType();
            TypeCodes globalCode; if (collection.IsGenericType)
            {
                globalCode = TypeFormatter.GetTypeCodes(collection.GetGenericArguments()[0]);
            }
            else globalCode = TypeCodes.Object;
            bool isExplicitType = (globalCode != TypeCodes.Object);

            serializationStream.Put((byte)globalCode);
            foreach (object item in value)
            {
                if (isExplicitType)
                {
                    TypeFormatter.Serialize(serializationStream, 0, item, false);
                }
                else TypeFormatter.Serialize(serializationStream, item);
            }
        }

        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <param name="fieldType">The underlaying field type to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static ICollection Read(Stream serializationStream, Type fieldType)
        {
            int count = (int)serializationStream.ToVariableInt();

            ICollection value = fieldType.CreateInstance<ICollection>();
            TypeCodes globalCode = (TypeCodes)serializationStream.Get();
            Type parameterType; if (fieldType.IsGenericType)
            {
                parameterType = fieldType.GetGenericArguments()[0];
            }
            else parameterType = typeof(object);
            bool isExplicitType = (globalCode != TypeCodes.Object);

            MethodInfo mi = fieldType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[] { parameterType }, null);
            for (; count > 0; count--)
            {
                object tmp; if (isExplicitType)
                {
                    tmp = TypeFormatter.Deserialize(serializationStream, (UInt32)globalCode, parameterType);
                }
                else tmp = TypeFormatter.Deserialize(serializationStream);
                if (mi != null)
                {
                    mi.Invoke(value, new object[] { tmp });
                }
            }
            return value;
        }
    }
}
