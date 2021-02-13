// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    public static partial class TypeExtension
    {
        /// <summary>
        /// Initializes this type if necessary
        /// </summary>
        public static void Initialize(this Type type)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}
            