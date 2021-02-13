// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    /// <summary>
    /// Defines valid JSON DOM nodes
    /// </summary>
    public enum SyntaxNodeType : byte
    {
        Empty = 0,
        
        UsingNamespace,
        Namespace,
    }
}