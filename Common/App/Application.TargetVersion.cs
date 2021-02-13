// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

namespace System.Runtime
{
    public static partial class Application
    {
        internal const int VersionShift = 4;
        private static VersionFlags targetVersion;

        /// <summary>
        /// Obtains a flag to the .NET version this application runs on
        /// </summary>
        public static VersionFlags TargetVersion
        {
            get
            {
                if (targetVersion == VersionFlags.Undefined)
                    lock (rootPath)
                    {
                        if (targetVersion == VersionFlags.Undefined)
                        {
                            Version version = null;
                            if (version == null)
                            {
                                GetFromAttribute(ref version);
                            }
                            if (version == null)
                            {
                                GetFromAssembly(ref version);
                            }
                            if (version != null)
                            {
                                switch (version.Minor)
                                {
                                    case 8:
                                        {
                                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_8 << VersionShift) | VersionFlags.NetFramework;
                                        }
                                        break;
                                    case 7: switch (version.Build)
                                        {
                                            case 2:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_7_2 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                            case 1:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_7_1 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                            case -1:
                                            case 0:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_7 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                        }
                                        break;
                                    case 6: switch (version.Build)
                                        {
                                            case 2:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_6_2 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                            case 1:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_6_1 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                            case -1:
                                            case 0:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_6 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                        }
                                        break;
                                    case 5: switch (version.Build)
                                        {
                                            case 2:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_5_2 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                            case 1:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_5_1 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                            case 0:
                                            case -1:
                                                {
                                                    targetVersion = (VersionFlags)((int)VersionFlags.Net4_5 << VersionShift) | VersionFlags.NetFramework;
                                                }
                                                break;
                                        }
                                        break;
                                    case 0:
                                        {
                                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_0 << VersionShift) | VersionFlags.NetFramework;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                return targetVersion;
            }
        }
        private static void GetFromAttribute(ref Version version)
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), true);
            if (attributes.Length > 0)
            {
                version = new FrameworkName((attributes[0] as TargetFrameworkAttribute).FrameworkName).Version;
            }
        }
        private static void GetFromAssembly(ref Version version)
        {
            string[] parts = FileVersionInfo.GetVersionInfo(typeof(int).Assembly.Location).ProductVersion.Split('.');
            int major; if (parts.Length > 0)
            {
                int.TryParse(parts[0], out major);
            }
            else major = 0;
            int minor; if (parts.Length > 1)
            {
                int.TryParse(parts[1], out minor);
            }
            else minor = 0;
            int build; if (parts.Length > 2 && parts[2].Length > 0)
            {
                int.TryParse(parts[2][0].ToString(), out build);
            }
            else build = 0;
            if (major >= 4 && major <= 5)
            {
                version = new Version(major, minor, build);
            }
        }

        /// <summary>
        /// Obtains a flag to the .NET version defined at compilation
        /// </summary>
        public static VersionFlags CompiledVersion
        {
            get
            {
                if (targetVersion == VersionFlags.Undefined)
                    lock (rootPath)
                    {
                        if (targetVersion == VersionFlags.Undefined)
                        {
                            #if net48
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_8 << VersionShift) | VersionFlags.NetFramework;
                            #elif net472
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_7_2 << VersionShift) | VersionFlags.NetFramework;
                            #elif net471
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_7_1 << VersionShift) | VersionFlags.NetFramework;
                            #elif net47
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_7 << VersionShift) | VersionFlags.NetFramework;
                            #elif net462
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_6_2 << VersionShift) | VersionFlags.NetFramework;
                            #elif net461
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_6_1 << VersionShift) | VersionFlags.NetFramework;
                            #elif net46
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_6 << VersionShift) | VersionFlags.NetFramework;
                            #elif net452
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_5_2 << VersionShift) | VersionFlags.NetFramework;
                            #elif net451
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_5_1 << VersionShift) | VersionFlags.NetFramework;
                            #elif net45
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_7 << VersionShift) | VersionFlags.NetFramework;
                            #elif net40
                            targetVersion = (VersionFlags)((int)VersionFlags.Net4_0 << VersionShift) | VersionFlags.NetFramework;
                            #endif
                        }
                    }
                return targetVersion;
            }
        }
    }
}
