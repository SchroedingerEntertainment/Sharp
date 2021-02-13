// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Specifies the type of serialized data
    /// </summary>
    public enum TypeCodes : uint
    {
        Empty = TypeCode.Empty,
        
        Boolean = TypeCode.Boolean,
        TrueConstant = 30,
        FalseConstant = 31,

        Byte = TypeCode.Byte,
        SByte = TypeCode.SByte,
        Char = TypeCode.Char,

        Int16 = TypeCode.Int16,
        UInt16 = TypeCode.UInt16,
        Int32 = TypeCode.Int32,
        UInt32 = TypeCode.UInt32,
        Int64 = TypeCode.Int64,
        UInt64 = TypeCode.UInt64,
        Single = TypeCode.Single,
        Double = TypeCode.Double,
        Decimal = TypeCode.Decimal,

        DateTime = TypeCode.DateTime,
        String = TypeCode.String,

        Guid = 32,

        Object = 39,
        Array = 40,
        Collection = 41,
        Dictionary = 42,

        Exception = 48,

        Marker = 70
    }
}
