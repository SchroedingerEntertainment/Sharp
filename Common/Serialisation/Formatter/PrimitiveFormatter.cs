// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides base functionality for the primitive serialization formatter
    /// </summary>
    public static class PrimitiveFormatter
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
        public static void WriteBoolean(Stream serializationStream, bool value)
        {
            if (value)
            {
                serializationStream.Put((byte)TypeCodes.TrueConstant);
            }
            else serializationStream.Put((byte)TypeCodes.FalseConstant);
        }

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
        public static void WriteByte(Stream serializationStream, byte value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Byte);
            }
            serializationStream.Put(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static byte ReadByte(Stream serializationStream)
        {
            return serializationStream.Get();
        }

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
        public static void WriteSByte(Stream serializationStream, SByte value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.SByte);
            }
            serializationStream.Put((byte)value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static SByte ReadSByte(Stream serializationStream)
        {
            return (SByte)serializationStream.Get();
        }

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
        public static void WriteChar(Stream serializationStream, char value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Char);
            }
            serializationStream.Encode((Int16)value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static char ReadChar(Stream serializationStream)
        {
            return (char)serializationStream.ToInt16();
        }

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
        public static void WriteInt16(Stream serializationStream, Int16 value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Int16);
            }
            serializationStream.Encode(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static Int16 ReadInt16(Stream serializationStream)
        {
            return serializationStream.ToInt16();
        }

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
        public static void WriteUInt16(Stream serializationStream, UInt16 value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.UInt16);
            }
            serializationStream.Encode(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static UInt16 ReadUInt16(Stream serializationStream)
        {
            return serializationStream.ToUInt16();
        }

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
        public static void WriteInt32(Stream serializationStream, Int32 value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Int32);
            }
            serializationStream.Encode(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static Int32 ReadInt32(Stream serializationStream)
        {
            return serializationStream.ToInt32();
        }

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
        public static void WriteUInt32(Stream serializationStream, UInt32 value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.UInt32);
            }
            serializationStream.Encode(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static UInt32 ReadUInt32(Stream serializationStream)
        {
            return serializationStream.ToUInt32();
        }

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
        public static void WriteInt64(Stream serializationStream, Int64 value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Int64);
            }
            serializationStream.Encode(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static Int64 ReadInt64(Stream serializationStream)
        {
            return serializationStream.ToInt64();
        }

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
        public static void WriteUInt64(Stream serializationStream, UInt64 value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.UInt64);
            }
            serializationStream.Encode(value);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static UInt64 ReadUInt64(Stream serializationStream)
        {
            return serializationStream.ToUInt64();
        }

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
        public static void WriteSingle(Stream serializationStream, float value, bool addTypeCode)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                SwapBufferData(buffer);
            }
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Single);
            }
            serializationStream.Write(buffer);
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static float ReadSingle(Stream serializationStream)
        {
            byte[] buffer = new byte[4];
            serializationStream.Read(buffer, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                SwapBufferData(buffer);
            }
            return BitConverter.ToSingle(buffer, 0);
        }

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
        public static void WriteDouble(Stream serializationStream, double value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Double);
            }
            serializationStream.Encode(BitConverter.DoubleToInt64Bits(value));
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static double ReadDouble(Stream serializationStream)
        {
            return BitConverter.Int64BitsToDouble(serializationStream.ToInt64());
        }

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
        public static void WriteDecimal(Stream serializationStream, decimal value, bool addTypeCode)
        {
            int[] buffer = Decimal.GetBits(value);
            if (buffer.Length > byte.MaxValue)
            {
                throw new OverflowException();
            }
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Decimal);
            }
            serializationStream.Put((byte)(buffer.Length & 0xFF));
            for (int i = 0; i < buffer.Length; i++)
            {
                serializationStream.Encode(buffer[i]);
            }
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static decimal ReadDecimal(Stream serializationStream)
        {
            int[] buffer = new int[serializationStream.Get()];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = serializationStream.ToInt32();
            }
            return new Decimal(buffer);
        }

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
        public static void WriteDateTime(Stream serializationStream, DateTime value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.DateTime);
            }
            serializationStream.Encode(value.ToBinary());
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static DateTime ReadDateTime(Stream serializationStream)
        {
            return DateTime.FromBinary(serializationStream.ToInt64());
        }

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
        public static void WriteGuid(Stream serializationStream, Guid value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Guid);
            }
            string tmp = value.ToString();
            serializationStream.Put((byte)tmp.Length);
            for (int i = 0; i < tmp.Length; i++)
            {
                serializationStream.Put((byte)tmp[i]);
            }
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static Guid ReadGuid(Stream serializationStream)
        {
            int length = serializationStream.Get();
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append((char)serializationStream.Get());
            }
            return Guid.Parse(sb.ToString());
        }

        private static void SwapBufferData(byte[] buffer)
        {
            byte tmp = buffer[0];
            buffer[0] = buffer[3];
            buffer[3] = tmp;
            tmp = buffer[1];
            buffer[1] = buffer[2];
            buffer[2] = tmp;
        }
    }
}