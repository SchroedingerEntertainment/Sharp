// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Defines properties of the parser
    /// </summary>
    public enum PropertyPageFlags : byte
    {
        Default = 0,

        DashNotation = 0x1,
        SlashNotation = 0x2,

        HarmonizeFlags = 0x4,

        AdditionalLineBreaks = 0x8
    }
}
