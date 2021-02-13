// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides base functionality for the Array serialization formatter
    /// </summary>
    public static class ArrayFormatter
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
        public static void Write(Stream serializationStream, Array value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Array);
            }
            serializationStream.EncodeVariableInt((UInt32)value.Rank);
            for (int i = 0; i < value.Rank; i++)
            {
                serializationStream.EncodeVariableInt((UInt32)value.GetLength(i));
            }
            TypeCodes globalCode = TypeFormatter.GetTypeCodes(value.GetType().GetElementType());
            bool isExplicitType = (globalCode != TypeCodes.Object);

            serializationStream.Put((byte)globalCode);
            Serialize(serializationStream, value, 0, new int[value.Rank], !isExplicitType);
        }
        private static void Serialize(Stream serializationStream, Array value, int rank, int[] address, bool addTypeCode)
        {
            bool writeValue = ((rank + 1) == value.Rank);
            for (; address[rank] < value.GetLength(rank); address[rank]++)
            {
                if (writeValue)
                {
                    if (!addTypeCode)
                    {
                        TypeFormatter.Serialize(serializationStream, 0, value.GetValue(address), false);
                    }
                    else TypeFormatter.Serialize(serializationStream, value.GetValue(address));
                }
                else
                {
                    Serialize(serializationStream, value, rank + 1, address, addTypeCode);
                    address[rank + 1] = 0;
                }
            }
        }

        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <param name="fieldType">The underlaying field type to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static Array Read(Stream serializationStream, Type fieldType)
        {
            int[] length = new int[serializationStream.ToVariableInt()];
            for (int i = 0; i < length.Length; i++)
            {
                length[i] = (int)serializationStream.ToVariableInt();
            }
            TypeCodes globalCode = (TypeCodes)serializationStream.Get();
            if (fieldType == typeof(object))
            {
                switch (globalCode)
                {
                    case TypeCodes.Boolean: fieldType = typeof(bool); break;
                    case TypeCodes.Byte: fieldType = typeof(byte); break;
                    case TypeCodes.SByte: fieldType = typeof(sbyte); break;
                    case TypeCodes.Char: fieldType = typeof(char); break;
                    case TypeCodes.Int16: fieldType = typeof(Int16); break;
                    case TypeCodes.UInt16: fieldType = typeof(UInt16); break;
                    case TypeCodes.Int32: fieldType = typeof(Int32); break;
                    case TypeCodes.UInt32: fieldType = typeof(UInt32); break;
                    case TypeCodes.Int64: fieldType = typeof(Int64); break;
                    case TypeCodes.UInt64: fieldType = typeof(UInt64); break;
                    case TypeCodes.Single: fieldType = typeof(Single); break;
                    case TypeCodes.Double: fieldType = typeof(Double); break;
                    case TypeCodes.Decimal: fieldType = typeof(Decimal); break;
                    case TypeCodes.DateTime: fieldType = typeof(DateTime); break;
                    case TypeCodes.String: fieldType = typeof(String); break;
                    case TypeCodes.Guid: fieldType = typeof(Guid); break;
                    default: throw new TypeLoadException(string.Format("Unable to deduce array type for '{0}'", globalCode));
                }
            }
            else fieldType = fieldType.GetElementType();
            Array value = Array.CreateInstance(fieldType, length);
            length[0] = 0;

            bool isExplicitType = (globalCode != TypeCodes.Object);

            Deserialize(serializationStream, value, 0, length, isExplicitType, globalCode, value.GetType().GetElementType());
            return value;
        }
        private static void Deserialize(Stream serializationStream, Array value, int rank, int[] address, bool isExplicitType, TypeCodes globalCode, Type elementType)
        {
            bool readValue = ((rank + 1) == value.Rank);
            for (; address[rank] < value.GetLength(rank); address[rank]++)
            {
                if (readValue)
                {
                    if (isExplicitType)
                    {
                        value.SetValue(TypeFormatter.Deserialize(serializationStream, (UInt32)globalCode, elementType), address);
                    }
                    else value.SetValue(TypeFormatter.Deserialize(serializationStream), address);
                }
                else
                {
                    address[rank + 1] = 0;
                    Deserialize(serializationStream, value, rank + 1, address, isExplicitType, globalCode, elementType);
                }
            }
        }
    }
}
