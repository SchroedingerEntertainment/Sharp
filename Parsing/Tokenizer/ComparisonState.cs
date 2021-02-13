// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// Used to determine next character in keyword comparison rules
    /// </summary>
    public enum ComparisonState : byte
    {
        Failure = Byte.MaxValue,
        Next = Failure - 1
    }
}
