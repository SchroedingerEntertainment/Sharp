// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Stores the given integer into this stream in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this Stream data, Int16 value)
        {
            data.Put((byte)((value >> 8) & 0xff));
            data.Put((byte)(value & 0xff));
        }
        /// <summary>
        /// Stores the given integer into this stream in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this Stream data, UInt16 value)
        {
            data.Put((byte)((value >> 8) & 0xff));
            data.Put((byte)(value & 0xff));
        }
        /// <summary>
        /// Stores the given integer into this stream in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this Stream data, Int32 value)
        {
            data.Put((byte)((value >> 24) & 0xff));
            data.Put((byte)((value >> 16) & 0xff));
            data.Put((byte)((value >> 8) & 0xff));
            data.Put((byte)(value & 0xff));
        }
        /// <summary>
        /// Stores the given integer into this stream in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this Stream data, UInt32 value)
        {
            data.Put((byte)((value >> 24) & 0xff));
            data.Put((byte)((value >> 16) & 0xff));
            data.Put((byte)((value >> 8) & 0xff));
            data.Put((byte)(value & 0xff));
        }
        /// <summary>
        /// Stores the given integer into this stream in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this Stream data, Int64 value)
        {
            Encode(data, (UInt32)(value >> 32));
            Encode(data, (UInt32)(value & 0xffffffff));
        }
        /// <summary>
        /// Stores the given integer into this stream in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this Stream data, UInt64 value)
        {
            Encode(data, (UInt32)(value >> 32));
            Encode(data, (UInt32)(value & 0xffffffff));
        }

        /// <summary>
        /// Stores the given integer into this stream
        /// </summary>
        /// <param name="value">An integer to store</param>
        public static void EncodeVariableInt(this Stream data, UInt32 value)
        {
            do
            {
                byte lower7bits = (byte)(value & 0x7f);
                value >>= 7;

                if (value > 0)
                {
                    lower7bits |= 128;
                }
                data.Put(lower7bits);
            }
            while (value > 0);
        }

        /// <summary>
        /// Reads the given integer from this stream in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static Int16 ToInt16(this Stream data)
        {
            return (Int16)(data.Get() << 8 | data.Get());
        }
        /// <summary>
        /// Reads the given integer from this stream in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static UInt16 ToUInt16(this Stream data)
        {
            return (UInt16)(data.Get() << 8 | data.Get());
        }
        /// <summary>
        /// Reads the given integer from this stream in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static Int32 ToInt32(this Stream data)
        {
            Int32 result = data.Get();
            result = (result << 8) | data.Get();
            result = (result << 8) | data.Get();
            result = (result << 8) | data.Get();
            return result;
        }
        /// <summary>
        /// Reads the given integer from this stream in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static UInt32 ToUInt32(this Stream data)
        {
            UInt32 result = data.Get();
            result = (result << 8) | data.Get();
            result = (result << 8) | data.Get();
            result = (result << 8) | data.Get();
            return result;
        }
        /// <summary>
        /// Reads the given integer from this stream in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static Int64 ToInt64(this Stream data)
        {
            UInt32 hipart = ToUInt32(data);
            UInt32 lopart = ToUInt32(data);
            return (((Int64)hipart) << 32) | lopart;
        }
        /// <summary>
        /// Reads the given integer from this stream in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static UInt64 ToUInt64(this Stream data)
        {
            UInt32 hipart = ToUInt32(data);
            UInt32 lopart = ToUInt32(data);
            return (((UInt64)hipart) << 32) | lopart;
        }

        /// <summary>
        /// Reads the given integer from this stream
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this stream</returns>
        public static UInt32 ToVariableInt(this Stream data)
        {
            UInt32 value = 0;
            for (int i = 0, shift = 0; ; i++)
            {
                byte lower7bits = data.Get();
                value |= (UInt32)((lower7bits & 0x7f) << shift);
                shift += 7;

                if ((lower7bits & 128) == 0)
                    break;
            }
            return value;
        }
    }
}
