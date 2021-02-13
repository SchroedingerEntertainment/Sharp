// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Json
{
    /// <summary>
    /// Defines valid JSON parser rules
    /// </summary>
    public enum ParserState : byte
    {
        Master = 0,

        Object,
        Array,

        Property,
        Value,

        Failure
    }
}
