// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Json
{
    /// <summary>
    /// Defines valid JSON tokens
    /// </summary>
    public enum Token : byte
    {
        Null = 19,
        True = 18,
        False = 17,
        Numeric = 16,
        String = 15,

        EndArray = 13,
        BeginArray = 12,

        EndObject = 8,
        BeginObject = 7,

        Colon = 4,
        Comma = 3,

        Whitespace = 1,
        Invalid = 0
    }
}
