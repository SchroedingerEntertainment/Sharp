// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Copies length bytes from current position of the stream into
        /// target stream
        /// </summary>
        /// <param name="target">Target stream to copy data to</param>
        /// <param name="length">The ammount of bytes to copy</param>
        public static void CopyRange(this Stream s, Stream target, int length)
        {
            int read;
            byte[] buffer = new byte[128];
            while (length > 0 && (read = s.Read(buffer, 0, Math.Min(buffer.Length, length))) > 0)
            {
                target.Write(buffer, 0, read);
                length -= read;
            }
        }
    }
}
