// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.Alchemy
{
    public partial class Tokenizer
    {
        Token ReadGenericTokens()
        {
            switch (GetCharacter())
            {
                case '/': switch (GetCharacter())
                    {
                        #region SingleLineComment = '//'(~'\\\n') *;
                        case '/':
                            {
                                if (Rules.SingleLineComment(this))
                                    return Token.Comment;
                            }
                            break;
                        #endregion

                        #region MultiLineComment = '/*'(~'*/') * '*/' ?;
                        case '*':
                            {
                                if (Rules.MultiLineComment(this))
                                    return Token.Comment;
                            }
                            break;
                        #endregion
                    }
                    goto default;

                #region DoubleQuotationLiteral = '\"' ('\\"' | ~'\"')* '\"';
                case '\"':
                    {
                        if (Rules.DoubleQuotationLiteral(this))
                        {
                            return Token.DoubleQuotationLiteral;
                        }
                        else return Token.BogusDoubleQuotationLiteral;
                    }
                #endregion

                #region SingleQuotationLiteral = '\'' ('\\'' | ~'\'')+ '\'';
                case '\'':
                    {
                        if (Rules.SingleQuotationLiteral(this))
                        {
                            return Token.SingleQuotationLiteral;
                        }
                        else return Token.BogusSingleQuotationLiteral;
                    }
                #endregion

                #region Identifier = (['a','z'] | ['A','Z']) (['a','z'] | ['A','Z'] | ['0','9'] | '_' | '-')*;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    {
                        Rules.Identifier(this);
                        switch (PreviewBufferedData())
                        {
                            #region Identifier
                            default: return Token.Identifier;
                                #endregion
                        }
                    }
                #endregion

                #region Numeric = (['0', '9'] | '.') ((('E' | 'e') ('+' | '-')) | ['a','z'] | ['A','Z'] | ['0','9'] '.')*;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                Numeric:
                    {
                        Rules.Numeric(this);
                        return Token.Numeric;
                    }
                #endregion

                #region LeftBrace {
                case '{':
                    {
                        return Token.LeftCurlyBracket;
                    }
                #endregion

                #region RightBrace }
                case '}':
                    {
                        return Token.RightCurlyBracket;
                    }
                #endregion

                #region LeftParentheses (
                case '(':
                    {
                        return Token.LeftRoundBracket;
                    }
                #endregion

                #region RightParentheses )
                case ')':
                    {
                        return Token.RightRoundBracket;
                    }
                #endregion

                #region LeftBracket [
                case '[':
                    {
                        return Token.LeftSquareBracket;
                    }
                #endregion

                #region RightBracket ]
                case ']':
                    {
                        return Token.RightSquareBracket;
                    }
                #endregion

                case '.': switch(PeekCharacter())
                    {
                        #region Numeric
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9': goto Numeric;
                        #endregion

                        #region VariableArgs (...)
                        case '.':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    case '.':
                                        {
                                            if (PeekCharacter() == '.')
                                            {
                                                RawDataBuffer.Position++;
                                                return Token.VariableArgs;
                                            }
                                            RawDataBuffer.Position--;
                                        }
                                        break;
                                }
                                RawDataBuffer.Position--;
                            }
                            break;
                        #endregion
                    }
                    goto default;

                case '!': switch(PeekCharacter())
                    {
                        #region NotEqual (!=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.NotEqual;
                            }
                        #endregion

                        #region LogicalNot (!)
                        default:
                            {
                                return Token.LogicalNot;
                            }
                        #endregion
                    }

                case '<': switch(PeekCharacter())
                    {
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                switch (PeekCharacter())
                                {
                                    #region ThreeWayComparsion (<=>)
                                    case '>':
                                        {
                                            RawDataBuffer.Position++;
                                            return Token.ThreeWayComparsion;
                                        }
                                    #endregion

                                    #region LessEqual (<=)
                                    default:
                                        {
                                            return Token.LessEqual;
                                        }
                                    #endregion
                                }
                            }

                        #region LessThan (<)
                        default:
                            {
                                return Token.LessThan;
                            }
                        #endregion
                    }

                case '>': switch(PeekCharacter())
                    {
                        #region GreaterEqual (>=)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.GreaterEqual;
                            }
                        #endregion

                        #region GreaterThan (>)
                        default:
                            {
                                return Token.GreaterThan;
                            }
                        #endregion
                    }

                case '&': switch(PeekCharacter())
                    {
                        #region LogicalAnd (&&)
                        case '&':
                            {
                                RawDataBuffer.Position++;
                                return Token.LogicalAnd;
                            }
                        #endregion

                        #region BitwiseAnd (&)
                        default:
                            {
                                return Token.BitwiseAnd;
                            }
                        #endregion
                    }

                #region BitwiseXor (^)
                case '^':
                    {
                        return Token.BitwiseXor;
                    }
                #endregion

                #region BitwiseNot (~)
                case '~':
                    {
                        return Token.BitwiseNot;
                    }
                #endregion

                case '|': switch(PeekCharacter())
                    {
                        #region LogicalOr (||)
                        case '|':
                            {
                                RawDataBuffer.Position++;
                                return Token.LogicalOr;
                            }
                        #endregion

                        #region BitwiseOr (|)
                        default:
                            {
                                return Token.BitwiseOr;
                            }
                        #endregion
                    }

                case '=': switch(PeekCharacter())
                    {
                        #region Equal (==)
                        case '=':
                            {
                                RawDataBuffer.Position++;
                                return Token.Equal;
                            }
                        #endregion
                    }
                    goto default;

                case '#': switch(PeekCharacter())
                    {
                        #region Concatenation (##)
                        case '#':
                            {
                                RawDataBuffer.Position++;
                                return Token.Concat;
                            }
                        #endregion

                        #region Stringify (#)
                        default:
                            {
                                return Token.Stringify;
                            }
                        #endregion
                    }

                #region Comma (,)
                case ',':
                    {
                        return Token.Comma;
                    }
                #endregion

                #region Whitespace;
                case '\r':
                case Char32.NewLineGroup.NextLine:
                case Char32.NewLineGroup.LineSeparator:
                case Char32.NewLineGroup.ParagraphSeparator:
                case Char32.WhiteSpaceGroup.Space:
                case Char32.WhiteSpaceGroup.HorizontalTab:
                case Char32.WhiteSpaceGroup.VerticalTab:
                case Char32.WhiteSpaceGroup.FormFeed:
                case Char32.WhiteSpaceGroup.NoBreakingSpace:
                case Char32.WhiteSpaceGroup.OghamSpace:
                case Char32.WhiteSpaceGroup.MongolianVowelSeparator:
                case Char32.WhiteSpaceGroup.EnQuad:
                case Char32.WhiteSpaceGroup.EmQuad:
                case Char32.WhiteSpaceGroup.EnSpace:
                case Char32.WhiteSpaceGroup.EmSpace:
                case Char32.WhiteSpaceGroup.ThreePerEmSpace:
                case Char32.WhiteSpaceGroup.FourPerEmSpace:
                case Char32.WhiteSpaceGroup.SixPerEmSpace:
                case Char32.WhiteSpaceGroup.PunctuationSpace:
                case Char32.WhiteSpaceGroup.ThinSpace:
                case Char32.WhiteSpaceGroup.HairSpace:
                case Char32.WhiteSpaceGroup.NarrowSpace:
                case Char32.WhiteSpaceGroup.IdeographicSpace:
                case Char32.WhiteSpaceGroup.MediumMathematicalSpace:
                    {
                        BaseRules.NullLiteral(this);
                        return Token.Whitespace;
                    }
                #endregion

                #region NewLine = '\n';
                case Char32.NewLineGroup.LineFeed:
                    {
                        State.Reset();
                        return Token.NewLine;
                    }
                #endregion

                #region Character
                default:
                    {
                        return Token.Character;
                    }
                #endregion
            }
        }
    }
}
