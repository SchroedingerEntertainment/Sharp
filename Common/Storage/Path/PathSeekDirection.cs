// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    /// <summary>
    /// Flags pointing to the seek direction in file system traversal
    /// </summary>
    public enum PathSeekOptions : byte
    {
        Forward = 0x1,
        Backward = 0x2,
        RootLevel = 0x4
    }
}
