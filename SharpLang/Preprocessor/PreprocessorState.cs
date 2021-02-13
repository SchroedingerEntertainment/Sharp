// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    public enum PreprocessorStates : byte
    {
        Master = 0,
        
        If,
        Elif,
        Else,
        Endif,
        Define,
        Undef,
        Error,
        Warning,
        Line,
        Region,
        Endregion,
        Pragma,
        
        Failure
    }
}