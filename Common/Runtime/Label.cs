// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public struct Label
    {
        readonly System.Reflection.Emit.Label label;
        
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
        public bool IsSet
        {
            get { return (label != null); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="name"></param>
        public Label(System.Reflection.Emit.Label label, string name)
        {
            this.label = label;
            this.name = name;
        }

        public static implicit operator System.Reflection.Emit.Label(Label label)
        {
            return label.label;
        }
    }
}
