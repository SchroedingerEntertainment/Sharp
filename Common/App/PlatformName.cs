// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    /// <summary>
    /// Defines possible platforms to detect
    /// </summary>
    public enum PlatformName : byte
    {
        Undefined = 0,
        Windows = 0x1,

        Unix = 0x2,
        Linux = 0x4,
        Mac = 0x8
    }
}
