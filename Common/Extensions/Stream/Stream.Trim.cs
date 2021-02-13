// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Trims the contents of this stream by the given amount of bytes
        /// </summary>
        /// <param name="count">The amount of bytes to remove from the top of the stream</param>
        public static void Trim(this MemoryStream ms, int count)
        {
            byte[] buffer = ms.GetBuffer();
            Buffer.BlockCopy(buffer, count, buffer, 0, (int)(ms.Length - count));
            ms.SetLength(ms.Length - count);
        }
    }
}