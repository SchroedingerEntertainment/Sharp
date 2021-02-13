// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace System.Runtime
{
    public static partial class Application
    {
        private readonly static PlatformID WindowsMask = (PlatformID.Win32NT | PlatformID.Win32S | PlatformID.Win32Windows | PlatformID.WinCE);

        private static PlatformName platform;
        /// <summary>
        /// The platform this code is currently running on
        /// </summary>
        public static PlatformName Platform
        {
            get
            {
                if (platform == PlatformName.Undefined)
                    lock (rootPath)
                    {
                        if (platform == PlatformName.Undefined)
                            platform = GetPlatform();
                    }
                return platform;
            }
        }

        private static PlatformTarget target;
        /// <summary>
        /// The target architecture this code is currently running on
        /// </summary>
        public static PlatformTarget Target
        {
            get
            {
                if (target == PlatformTarget.Undefined)
                    lock (rootPath)
                    {
                        if (target == PlatformTarget.Undefined)
                            target = GetTarget();
                    }
                return target;
            }
        }

        /// <summary>
        /// Detects current Operation System
        /// </summary>
        public static PlatformName GetPlatform()
        {
            if (Path.DirectorySeparatorChar == '\\' && (WindowsMask & Environment.OSVersion.Platform) == Environment.OSVersion.Platform)
            {
                return PlatformName.Windows;
            }
            else
            {
                string platformName = string.Empty;
                try
                {
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = "uname";
                    p.Start();

                    platformName = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    if (platformName == null)
                    {
                        platformName = string.Empty;
                    }
                    platformName = platformName.Trim();
                }
                catch
                { }

                if (platformName.Contains("Darwin"))
                {
                    return PlatformName.Unix | PlatformName.Mac;
                }
                else if (platformName.Contains("Linux"))
                {
                    return PlatformName.Unix | PlatformName.Linux;
                }
                else if (!string.IsNullOrWhiteSpace(platformName))
                {
                    return PlatformName.Unix;
                }
                else return PlatformName.Undefined;
            }
        }

        /// <summary>
        /// Detects current CPU architecture
        /// </summary>
        /// <returns></returns>
        public static PlatformTarget GetTarget()
        {
            switch (IntPtr.Size)
            {
                case 8: return PlatformTarget.x64;
                case 4: return PlatformTarget.x86;
                default: return PlatformTarget.Undefined;
            }
        }
    }
}