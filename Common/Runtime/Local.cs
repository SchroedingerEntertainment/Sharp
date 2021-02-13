// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace System.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public struct Local
    {
        readonly LocalBuilder local;
            
        readonly string name;
        /// <summary>
        /// 
        /// </summary>
        public string Name 
        {
            get { return name; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public Type Type 
        { 
            get { return local.LocalType; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSet
        {
            get { return (local != null); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localBuilder"></param>
        /// <param name="name"></param>
        public Local(LocalBuilder localBuilder, string name)
        {
            this.local = localBuilder;
            this.name = name;
        }

        public static implicit operator LocalBuilder(Local local)
        {
            return local.local;
        }
    }
}
