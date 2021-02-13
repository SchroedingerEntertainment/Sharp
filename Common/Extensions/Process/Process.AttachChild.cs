// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime;

namespace System.Diagnostics
{
    public static partial class ProcessExtension
    {
        public const string ProcessChildIdentifier = "SE_PROCESS_PARENT";

        /// <summary>
        /// Starts a process resource and associates it with a Process component.
        /// The started process contains a flag to identify this Processe's main module.
        /// </summary>
        /// <returns>
        /// true if a process resource is started; false if no new process resource 
        /// is started (for example, if an existing process is reused)
        /// </returns>
        public static bool AttachChild(this Process process)
        {
            process.StartInfo.EnvironmentVariables[ProcessChildIdentifier] = Application.Self.GetAbsolutePath();
            return process.Start();
        }
    }
}