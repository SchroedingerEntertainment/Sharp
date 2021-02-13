// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.CommandLine
{
    /// <summary>
    /// The list of valid preprocessor tokens
    /// </summary>
    public enum Token : ushort
    {
        QuotedStringLiteral = 12,
        StringLiteral = 11,

        Numeric = 9,

        LongIdentifier = 6,
        Identifier = 5,

        ResponseFile = 3,

        Separator = 2,
        Delimiter = 1,
        Invalid = 0,
    }
}
