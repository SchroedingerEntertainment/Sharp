// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.CommandLine
{
    /// <summary>
    /// Defines properties of the parser
    /// </summary>
    public enum CommandLineFlags : byte
    {
        Default = 0,
        AllowCompound = 0x1,
        AllowVerbValues = 0x2,
        IgnoreCase = 0x4
    }
}
