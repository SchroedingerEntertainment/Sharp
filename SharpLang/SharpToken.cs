// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    public enum SharpToken
    {
        Invalid = 0,

        #region Generic
        Identifier = Token.Identifier,

        VerbatimIdentifier = Token.VerbatimIdentifier,
        StringLiteral = Token.StringLiteral,
        RawStringLiteral = VerbatimIdentifier,

        InterpolatedStringLiteral = Token.InterpolatedStringLiteral,
        BogusStringLiteral = Token.BogusStringLiteral,

        CharacterLiteral = Token.CharacterLiteral,
        BogusCharacterLiteral = Token.BogusCharacterLiteral,

        Numeric = Token.Numeric,
        Character = Token.Character,

        New = Token.New,

        Hash = Token.Hash,

        LeftCurlyBracket = Token.LeftCurlyBracket,
        CurlyBracketOpen = LeftCurlyBracket,
        RightCurlyBracket = Token.RightCurlyBracket,
        CurlyBracketClose = RightCurlyBracket,

        LeftRoundBracket = Token.LeftRoundBracket,
        RoundBracketOpen = LeftRoundBracket,
        RightRoundBracket = Token.RightRoundBracket,
        RoundBracketClose = RightRoundBracket,

        LeftSquareBracket = Token.LeftSquareBracket,
        SquareBracketOpen = LeftSquareBracket,
        RightSquareBracket = Token.RightSquareBracket,
        SquareBracketClose = RightSquareBracket,

        Lamda = Token.Lamda,

        RestParams = Token.RestParams,
        Range = Token.Range,

        LogicalAnd = Token.LogicalAnd,
        LogicalOr = Token.LogicalOr,
        LessThan = Token.LessThan,
        LessEqual = Token.LessEqual,
        GreaterEqual = Token.GreaterEqual,
        GreaterThan = Token.GreaterThan,
        LogicalNot = Token.LogicalNot,
        NotEqual = Token.NotEqual,
        Equal = Token.Equal,

        LeftAngleBracket = LessThan,
        AngleBracketOpen = LessThan,
        RightAngleBracket = GreaterThan,
        AngleBracketClose = GreaterThan,

        BitwiseAnd = Token.BitwiseAnd,
        BitwiseAndAssign = Token.BitwiseAndAssign,
        BitwiseXor = Token.BitwiseXor,
        BitwiseXorAssign = Token.BitwiseXorAssign,
        BitwiseOr = Token.BitwiseOr,
        BitwiseOrAssign = Token.BitwiseOrAssign,
        BitwiseNot = Token.BitwiseNot,
        RightShift = Token.RightShift,
        RightShiftAssign = Token.RightShiftAssign,
        LeftShift = Token.LeftShift,
        LeftShiftAssign = Token.LeftShiftAssign,

        Mod = Token.Mod,
        ModAssign = Token.ModAssign,
        Multiple = Token.Multiple,
        MultipleAssign = Token.MultipleAssign,
        Div = Token.Div,
        DivAssign = Token.DivAssign,
        Decrement = Token.Decrement,
        Sub = Token.Sub,
        SubAssign = Token.SubAssign,
        Increment = Token.Increment,
        Add = Token.Add,
        AddAssign = Token.AddAssign,

        Assign = Token.Assign,
        NullCoalescing = Token.NullCoalescing,
        NullCoalescingAssign = Token.NullCoalescingAssign,
        Ternary = Token.Ternary,

        Semicolon = Token.Semicolon,
        Colon = Token.Colon,
        Dot = Token.Dot,
        Comma = Token.Comma,
        Whitespace = Token.Whitespace,
        #endregion

        #region Keywords
        KeywordsLowerEnd = Token.Custom,

        Abstract = KeywordsLowerEnd,
        As,
        Base,
        Bool,
        Break,
        Byte,
        Case,
        Catch,
        Char,
        Checked,
        Class,
        Const,
        Continue,
        Decimal,
        Default,
        Delegate,
        Do,
        Double,
        Else,
        Enum,
        Event,
        Explicit,
        Extern,
        False,
        Finally,
        Fixed,
        Float,
        For,
        Foreach,
        Goto,
        If,
        Implicit,
        In,
        Int,
        Interface,
        Internal,
        Is,
        Lock,
        Long,
        Namespace,
        Null,
        Object,
        Operator,
        Out,
        Override,
        Params,
        Private,
        Protected,
        Public,
        Readonly,
        Ref,
        Return,
        Sbyte,
        Sealed,
        Short,
        Sizeof,
        Stackalloc,
        Static,
        String,
        Struct,
        Switch,
        This,
        Throw,
        True,
        Try,
        Typeof,
        Uint,
        Ulong,
        Unchecked,
        Unsafe,
        Ushort,
        Using,
        Virtual,
        Void,
        Volatile,
        While,

        KeywordsUpperEnd = While,
        #endregion
    }
}
