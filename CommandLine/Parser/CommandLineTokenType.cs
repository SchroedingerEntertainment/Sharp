// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.CommandLine
{
    /// <summary>
    /// The type of a parsed command line argument
    /// </summary>
    public enum CommandLineTokenType
    {
        ParserGenerated = 0,
        Property,

        Bool,
        Integer,
        Decimal,
        String,

        PlainText
    }
}
