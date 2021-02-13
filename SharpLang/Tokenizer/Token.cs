// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.SharpLang
{
    public enum Token
    {
        Invalid,
        SingleLineComment,
        MultiLineComment,
        Identifier,
        VerbatimIdentifier,
        StringLiteral,
        RawStringLiteral = VerbatimIdentifier,
        InterpolatedStringLiteral,
        BogusStringLiteral,
        CharacterLiteral,
        BogusCharacterLiteral,
        Numeric,
        Character,
        
        IfDirective,
        ElifDirective,
        ElseDirective,
        EndifDirective,
        DefineDirective,
        UndefDirective,
        Line,
        Warning,
        Error,
        Region,
        Endregion,
        Pragma,
        BogusDirective,
        
        Empty,
        
        New,
        
        Hash,
        
        LeftCurlyBracket,
        CurlyBracketOpen = LeftCurlyBracket,
        RightCurlyBracket,
        CurlyBracketClose = RightCurlyBracket,
        
        LeftRoundBracket,
        RoundBracketOpen = LeftRoundBracket,
        RightRoundBracket,
        RoundBracketClose = RightRoundBracket,
        
        LeftSquareBracket,
        SquareBracketOpen = LeftSquareBracket,
        RightSquareBracket,
        SquareBracketClose = RightSquareBracket,
        
        Lamda,
        
        RestParams,
        Range,
        
        LogicalAnd,
        LogicalOr,
        LessThan,
        LessEqual,
        GreaterEqual,
        GreaterThan,
        LogicalNot,
        NotEqual,
        Equal,

        LeftAngleBracket = LessThan,
        AngleBracketOpen = LessThan,
        RightAngleBracket = GreaterThan,
        AngleBracketClose = GreaterThan,

        BitwiseAnd,
        BitwiseAndAssign,
        BitwiseXor,
        BitwiseXorAssign,
        BitwiseOr,
        BitwiseOrAssign,
        BitwiseNot,
        RightShift,
        RightShiftAssign,
        LeftShift,
        LeftShiftAssign,
        
        Mod,
        ModAssign,
        Multiple,
        MultipleAssign,
        Div,
        DivAssign,
        Decrement,
        Sub,
        SubAssign,
        Increment,
        Add,
        AddAssign,
        
        Assign,
        NullCoalescing,
        NullCoalescingAssign,
        Ternary,
        
        Semicolon,
        Colon,
        Dot,
        Comma,
        Whitespace,
        NewLine,

        Custom
    }
}