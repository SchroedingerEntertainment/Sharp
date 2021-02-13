// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.CommandLine
{
    /// <summary>
    /// Defines the states of the preprocessor
    /// </summary>
    public enum CommandLineParserState : byte
    {
        Initial = 0,

        SingleDashProperty,
        DoubleDashProperty,

        PropertyValue,
        VerbValue,

        PlainText,
        Failure
    }
}
