// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class TypeExtension
    {
        public readonly static Type IntPtrType = typeof(IntPtr);
        public readonly static Type UIntPtrType = typeof(UIntPtr);

        /// <summary>
        /// Determines if the type is a struct
        /// </summary>
        public static bool IsStruct(this Type type)
        {
            return 
            (
                type.IsValueType && 
               !type.IsPrimitive && 
               !type.IsEnum && 
                type != IntPtrType && 
                type != UIntPtrType
            );
        }
    }
}
