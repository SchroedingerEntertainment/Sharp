// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Defines the type of a property when meta information are
    /// collected for display
    /// </summary>
    public enum PropertyType : byte
    {
        Default,
        Optional,
        Required
    }
}
