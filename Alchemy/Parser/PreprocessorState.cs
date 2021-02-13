// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Alchemy
{
    /// <summary>
    /// Defines the states of the preprocessor
    /// </summary>
    public enum TextPreprocessorState : byte
    {
        Master = 0,

        Ifndef,
        Ifdef,
        If,
        Import,
        Error,
        Warning,
        Endif,
        Else,
        Elif,
        Undefine,
        Define,
        FunctionMacro,

        Failure
    }
}
