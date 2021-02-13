// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.Json
{
    public partial class Tokenizer
    {
        protected static class Rules
        {
            /// <summary>
            /// StringLiteral = '\"' ('\\"' | ~'\"')* '\"';
            /// </summary>
            public static bool StringLiteral(Tokenizer data)
            {
                for (bool skip = false; !data.EndOfStream;)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '\"':
                            {
                                if (!skip)
                                {
                                    data.RawDataBuffer.Buffer.RemoveAt(0);
                                    data.RawDataBuffer.Discard(1);
                                    return true;
                                }
                            }
                            break;
                        case '\n':
                            {
                                if (!skip)
                                    return false;
                            }
                            break;
                    }
                    skip = !skip && (c == '\\');
                    data.Position++;
                }
                return false;
            }

            /// <summary>
            /// Numeric = (('-')?['0', '9']) ((('E' | 'e') ('+' | '-')) | ['0','9'] '.')*
            /// </summary>
            public static bool Numeric(Tokenizer data)
            {
                for (int count = 0; ; count++)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '.':
                            {
                                data.Position++;
                            }
                            break;
                        case 'e':
                        case 'E':
                            {
                                data.Position++;
                                switch (data.PeekCharacter())
                                {
                                    case '+':
                                    case '-':
                                        {
                                            data.Position++;
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            {
                                if (!IsNumericChar(c))
                                {
                                    return (count > 0);
                                }
                                else data.Position++;
                            }
                            break;
                    }
                }
            }

            /// <summary>
            /// TrueConstant = 'true';
            /// </summary>
            public static bool TrueConstant(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'r') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'u') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// FalseConstant = 'false';
            /// </summary>
            public static bool FalseConstant(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'a') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'l') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 's') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }

            /// <summary>
            /// NullConstant = 'null';
            /// </summary>
            public static bool NullConstant(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'u') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                        case (ComparisonState)1:
                            {
                                if (c == 'l') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (!IsIdentifierChar(c)) return true;
                                else goto case ComparisonState.Failure;
                            }
                        default:
                        case ComparisonState.Failure: return false;
                        case ComparisonState.Next: state++; break;
                    }
                    data.Position++;
                }
            }
        }

        /// <summary>
        /// Determines if a character is a valid identifier character to this tokenizer
        /// </summary>
        public static bool IsIdentifierChar(Char32 c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        }

        /// <summary>
        /// Determines if a character is a valid numeric to this tokenizer
        /// </summary>
        public static bool IsNumericChar(Char32 c)
        {
            return (c >= '0' && c <= '9');
        }
    }
}
