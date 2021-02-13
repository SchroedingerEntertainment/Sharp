// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    /// <summary>
    /// Defines the available .NET runtime versions
    /// </summary>
    public enum VersionFlags : byte
    {
        Undefined = 0,
        
        NetFramework = 0x1,

        Net4_8 = 3,
        Net4_7_2 = 4,
        Net4_7_1 = 5,
        Net4_7 = 6,
        Net4_6_2 = 7,
        Net4_6_1 = 8,
        Net4_6 = 9,
        Net4_5_2 = 10,
        Net4_5_1 = 11,
        Net4_5 = 12,
        Net4_0 = 13,
    }
}
