// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.Alchemy
{
    public partial class Tokenizer
    {
        /// <summary>
        /// Base rules used by this tokenizer
        /// </summary>
        protected static class Rules
        {
            /// <summary>
            /// SingleLineComment = '//' (~'\\\n')*;
            /// </summary>
            public static bool SingleLineComment(Tokenizer data)
            {
                while (!data.EndOfStream && data.PeekCharacter() != '\n')
                    data.Position++;

                return true;
            }
            /// <summary>
            /// MultiLineComment = '/*' (~'*/')* '*/'?;
            /// </summary>
            public static bool MultiLineComment(Tokenizer data)
            {
                while (!data.EndOfStream)
                {
                    while (!data.EndOfStream && data.PeekCharacter() != '*')
                        data.Position++;

                    data.Position++;
                    if (data.PeekCharacter() == '/')
                        break;
                }

                data.Position++;
                return true;
            }

            /// <summary>
            /// DoubleQuotationLiteral = '\"' ('\\"' | ~'\"')* '\"';
            /// </summary>
            public static bool DoubleQuotationLiteral(Tokenizer data)
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
                                    data.Position++;
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
            /// SingleQuotationLiteral = '\'' ('\\'' | ~'\'')+ '\'';
            /// </summary>
            public static bool SingleQuotationLiteral(Tokenizer data)
            {
                for (bool skip = false; !data.EndOfStream;)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '\'':
                            {
                                if (!skip)
                                {
                                    data.Position++;
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
            /// Identifier = (['a','z'] | ['A','Z']) (['a','z'] | ['A','Z'] | ['0','9'] | '_' | '-')*;
            /// </summary>
            public static bool Identifier(Tokenizer data)
            {
                for (int count = 0; ; count++)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        default:
                            {
                                if (!IsIdentifierChar(c))
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
            /// Numeric = (['0', '9'] | '.') ((('E' | 'e') ('+' | '-')) | ['0','9'] '.')*;
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
            /// IfdefDirective = '#ifdef';
            /// </summary>
            public static bool IfdefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
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
            /// IfndefDirective = '#ifndef';
            /// </summary>
            public static bool IfndefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'd') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
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
            /// ImportDirective = '#import';
            /// </summary>
            public static bool ImportDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'p') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'o') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'r') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 't') goto case ComparisonState.Next;
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
            /// ElifDirective = '#elif';
            /// </summary>
            public static bool ElifDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
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
            /// ElseDirective = '#else';
            /// </summary>
            public static bool ElseDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
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
            /// ErrorDirective = '#error';
            /// </summary>
            public static bool ErrorDirective(Tokenizer data)
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
                                if (c == 'o') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'r') goto case ComparisonState.Next;
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
            /// EnableDirective = '#enable';
            /// </summary>
            public static bool EnableDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'b') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'l') goto case ComparisonState.Next;
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
            /// EndifDirective = '#endif';
            /// </summary>
            public static bool EndifDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'i') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
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
            /// DefineDirective = '#define';
            /// </summary>
            public static bool DefineDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'i') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'n') goto case ComparisonState.Next;
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
            /// DisableDirective = '#disable';
            /// </summary>
            public static bool DisableDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 's') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'a') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'b') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'l') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)4:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)5:
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
            /// UndefDirective = '#undef';
            /// </summary>
            public static bool UndefDirective(Tokenizer data)
            {
                ComparisonState state = 0;
                Char32 c; for (; ; )
                {
                    c = data.PeekCharacter();
                    switch (state)
                    {
                        case (ComparisonState)0:
                            {
                                if (c == 'n') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)1:
                            {
                                if (c == 'd') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'e') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)3:
                            {
                                if (c == 'f') goto case ComparisonState.Next;
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
            /// WarnDirective = '#warn';
            /// </summary>
            public static bool WarnDirective(Tokenizer data)
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
                                if (c == 'r') goto case ComparisonState.Next;
                                else goto case ComparisonState.Failure;
                            }
                        case (ComparisonState)2:
                            {
                                if (c == 'n') goto case ComparisonState.Next;
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
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_' || c == '-');
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
