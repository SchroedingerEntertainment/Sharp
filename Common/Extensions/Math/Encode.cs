// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class MathExtension
    {
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this byte[] data, Int16 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 8) & 0xff);
            data[offset + 1] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this byte[] data, UInt16 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 8) & 0xff);
            data[offset + 1] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this byte[] data, Int32 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 24) & 0xff);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this byte[] data, UInt32 value, int offset = 0)
        {
            data[offset] = (byte)((value >> 24) & 0xff);
            data[offset + 1] = (byte)((value >> 16) & 0xff);
            data[offset + 2] = (byte)((value >> 8) & 0xff);
            data[offset + 3] = (byte)(value & 0xff);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this byte[] data, Int64 value, int offset = 0)
        {
            Encode(data, (UInt32)(value >> 32), offset);
            Encode(data, (UInt32)(value & 0xffffffff), offset + 4);
        }
        /// <summary>
        /// Stores the given integer into this array in big endian byte order
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        public static void Encode(this byte[] data, UInt64 value, int offset = 0)
        {
            Encode(data, (UInt32)(value >> 32), offset);
            Encode(data, (UInt32)(value & 0xffffffff), offset + 4);
        }

        /// <summary>
        /// Stores the given integer into this array. The array must
        /// at least have 5 bytes left to encode
        /// </summary>
        /// <param name="value">An integer to store</param>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The amount of bytes written</returns>
        public static int EncodeVariableInt(this byte[] data, UInt32 value, int offset = 0)
        {
            int i = offset;
            do
            {
                byte lower7bits = (byte)(value & 0x7f);
                value >>= 7;
                
                if (value > 0)
                {
                    lower7bits |= 128;
                }
                data[i] = lower7bits;
                i++;
            } 
            while (value > 0);
            return (i - offset);
        }

        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static Int16 ToInt16(this byte[] data, int offset = 0)
        {
            return (Int16)(data[offset] << 8 | data[offset + 1]);
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt16 ToUInt16(this byte[] data, int offset = 0)
        {
            return (UInt16)(data[offset] << 8 | data[offset + 1]);
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static Int32 ToInt32(this byte[] data, int offset = 0)
        {
            Int32 result = data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset];
            return result;
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt32 ToUInt32(this byte[] data, int offset = 0)
        {
            UInt32 result = data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset++];
            result = (result << 8) | data[offset];
            return result;
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static Int64 ToInt64(this byte[] data, int offset = 0)
        {
            UInt32 hipart = ToUInt32(data, offset);
            UInt32 lopart = ToUInt32(data, offset + 4);
            return (((Int64)hipart) << 32) | lopart;
        }
        /// <summary>
        /// Reads the given integer from this array in platform byte order
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt64 ToUInt64(this byte[] data, int offset = 0)
        {
            UInt32 hipart = ToUInt32(data, offset);
            UInt32 lopart = ToUInt32(data, offset + 4);
            return (((UInt64)hipart) << 32) | lopart;
        }

        /// <summary>
        /// Reads the given integer from this array
        /// </summary>
        /// <param name="offset">An offset at which the integer should start from zero</param>
        /// <returns>The integer value stored in this array</returns>
        public static UInt32 ToVariableInt(this byte[] data, int offset = 0)
        {
            UInt32 value = 0;
            for (int i = offset, shift = 0; ; i++)
            {
                value |= (UInt32)((data[i] & 0x7f) << shift);
                shift += 7;

                if ((data[i] & 128) == 0)
                    break;
            }
            return value;
        }
    }
}
