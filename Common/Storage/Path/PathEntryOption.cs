// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.IO
{
    /// <summary>
    /// Flags determining the kind of file system entry to process
    /// </summary>
    [Flags]
    public enum PathEntryOption : byte
    {
        File = 0x1,
        Directory = 0x2
    }
}
