// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Alchemy
{
    /// <summary>
    /// The list of valid preprocessor tokens
    /// </summary>
    public enum Token : ushort
    {
        Comment = 145,

        Identifier = 140,

        DoubleQuotationLiteral = 136,
        BogusDoubleQuotationLiteral = 135,

        SingleQuotationLiteral = 131,
        BogusSingleQuotationLiteral = 130,

        Numeric = 127,

        Character = 124,

        IfDirective = 121,
        IfdefDirective = 120,
        IfndefDirective = 119,
        ElifDirective = 118,
        ElseDirective = 117,
        EndifDirective = 116,

        DefineDirective = 113,
        UndefDirective = 112,

        Error = 109,
        Warning = 108,

        ImportDirective = 104,

        DisableDirective = 97,
        EnableDirective = 96,

        /// <summary>
        /// (##)
        /// </summary>
        Concat = 93,
        /// <summary>
        /// (#)
        /// </summary>
        Stringify = 92,

        /// <summary>
        /// (##)
        /// </summary>
        DoubleHash = 86,
        /// <summary>
        /// (#)
        /// </summary>
        Hash = 85,

        /// <summary>
        /// {
        /// </summary>
        LeftCurlyBracket = 82,
        /// <summary>
        /// {
        /// </summary>
        CurlyBracketOpen = LeftCurlyBracket,

        /// <summary>
        /// }
        /// </summary>
        RightCurlyBracket = 80,
        /// <summary>
        /// }
        /// </summary>
        CurlyBracketClose = RightCurlyBracket,

        /// <summary>
        /// (
        /// </summary>
        LeftRoundBracket = 78,
        /// <summary>
        /// (
        /// </summary>
        RoundBracketOpen = LeftRoundBracket,

        /// <summary>
        /// )
        /// </summary>
        RightRoundBracket = 76,
        /// <summary>
        /// )
        /// </summary>
        RoundBracketClose = RightRoundBracket,

        /// <summary>
        /// [
        /// </summary>
        LeftSquareBracket = 74,
        /// <summary>
        /// [
        /// </summary>
        SquareBracketOpen = LeftSquareBracket,

        /// <summary>
        /// ]
        /// </summary>
        RightSquareBracket = 72,
        /// <summary>
        /// ]
        /// </summary>
        SquareBracketClose = RightSquareBracket,

        /// <summary>
        /// <
        /// </summary>
        LeftAngleBracket = LessThan,
        /// <summary>
        /// <
        /// </summary>
        AngleBracketOpen = LessThan,

        /// <summary>
        /// >
        /// </summary>
        RightAngleBracket = GreaterThan,
        /// <summary>
        /// >
        /// </summary>
        AngleBracketClose = GreaterThan,

        /// <summary>
        /// (...)
        /// </summary>
        VariableArgs = 63,

        /// <summary>
        /// (<=>)
        /// </summary>
        ThreeWayComparsion = 61,

        /// <summary>
        /// (&&)
        /// </summary>
        LogicalAnd = 59,

        /// <summary>
        /// (||)
        /// </summary>
        LogicalOr = 57,

        /// <summary>
        /// (<)
        /// </summary>
        LessThan = 55,
        /// <summary>
        /// (<=)
        /// </summary>
        LessEqual = 54,

        /// <summary>
        /// (>)
        /// </summary>
        GreaterEqual = 52,
        /// <summary>
        /// (>=)
        /// </summary>
        GreaterThan = 51,

        /// <summary>
        /// (!)
        /// </summary>
        LogicalNot = 49,
        /// <summary>
        /// (!=)
        /// </summary>
        NotEqual = 48,

        /// <summary>
        /// (==)
        /// </summary>
        Equal = 46,

        /// <summary>
        /// (&)
        /// </summary>
        BitwiseAnd = 44,

        /// <summary>
        /// (^)
        /// </summary>
        BitwiseXor = 41,

        /// <summary>
        /// (|)
        /// </summary>
        BitwiseOr = 38,

        /// <summary>
        /// (~)
        /// </summary>
        BitwiseNot = 35,

        /// <summary>
        /// (,)
        /// </summary>
        Comma = 5,

        Whitespace = 2,
        NewLine = 1,

        Invalid = 0,
    }
}
