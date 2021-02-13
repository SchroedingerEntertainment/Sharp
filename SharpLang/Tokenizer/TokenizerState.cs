// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    public enum TokenizerState : byte
    {
        Initial = 0,
        AfterWhitespace
    }
}