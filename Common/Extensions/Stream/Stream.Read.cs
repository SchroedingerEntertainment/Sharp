// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Reads the contents of the stream into the given array
        /// </summary>
        public static int Read(this Stream s, byte[] buffer)
        {
            return s.Read(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// Reads a single string of given length from this Stream
        /// </summary>
        /// <param name="length">The length to read data from this Stream</param>
        /// <returns>A string that contains this Streams data</returns>
        public static string Read(this Stream s, long length)
        {
            byte[] bt = new byte[length];
            s.Read(bt, 0, (int)length);

            return Encoding.UTF8.GetString(bt);
        }
    }
}
