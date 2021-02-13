// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class _void
    {
        internal readonly static Module PolicyModule = typeof(object).Assembly.ManifestModule;
        internal readonly static Type ProxyType = typeof(_void);
        internal readonly static Type Type = typeof(void);

        /// <summary>
        /// 
        /// </summary>
        private _void()
        { }
    }
}
