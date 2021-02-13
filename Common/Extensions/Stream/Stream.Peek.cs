// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Returns the next character from this Stream without processing it
        /// </summary>
        /// <returns>The character read from the stream or zero</returns>
        public static byte Peek(this Stream s)
        {
            if (s.Eof()) return 0;

            byte rt = (byte)s.ReadByte();
            s.Seek(-1, SeekOrigin.Current);

            return rt;
        }
    }
}