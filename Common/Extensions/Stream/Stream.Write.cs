// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Writes the contents of given array to this Stream
        /// </summary>
        public static void Write(this Stream s, byte[] buffer)
        {
            s.Write(buffer, 0, buffer.Length);
        }
    }
}
