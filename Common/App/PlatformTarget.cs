// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    /// <summary>
    /// Defines possible CPU architectures to detect
    /// </summary>
    public enum PlatformTarget : byte
    {
        Undefined = 0,
        Any,
        Arm,
        x86,
        Arm64,
        x64,
    }
}
