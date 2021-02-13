// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Determines if this Stream is at the end of data
        /// </summary>
        /// <returns>True if this Streams end is reached, false otherwise</returns>
        public static bool Eof(this Stream s)
        {
            return (s.Length <= s.Position);
        }
    }
}