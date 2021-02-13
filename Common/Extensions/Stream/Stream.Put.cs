﻿// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Writes a single byte onto the stream
        /// </summary>
        /// <param name="bt">The byte to be written</param>
        public static void Put(this Stream s, byte bt)
        {
            s.WriteByte(bt);
        }
    }
}
