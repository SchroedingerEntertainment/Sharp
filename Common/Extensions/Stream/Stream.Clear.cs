// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    public static partial class StreamExtension
    {
        /// <summary>
        /// Removes all contents from this stream
        /// </summary>
        public static void Clear(this Stream s)
        {
            s.Position = 0;
            s.SetLength(0);
        }
    }
}
