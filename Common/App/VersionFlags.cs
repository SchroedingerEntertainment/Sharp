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
        NetCore = 0x2,

        Net5_0 = 16,
        Net4_8 = 15,
        Net4_7_2 = 14,
        Net4_7_1 = 13,
        Net4_7 = 12,
        Net4_6_2 = 11,
        Net4_6_1 = 10,
        Net4_6 = 9,
        Net4_5_2 = 8,
        Net4_5_1 = 7,
        Net4_5 = 6,
        Net4_0 = 5
    }
}
