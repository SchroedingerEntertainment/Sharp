// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    public static partial class TargetVersionExtension
    {
        /// <summary>
        /// Returns the version ID component
        /// </summary>
        public static VersionFlags GetVersion(this VersionFlags flags)
        {
            return (VersionFlags)((int)flags >> Application.VersionShift);
        }
    }
}