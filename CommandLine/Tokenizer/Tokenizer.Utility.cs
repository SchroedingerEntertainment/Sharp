// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.CommandLine
{
    public partial class Tokenizer
    {
        /// <summary>
        /// Base rules used by this tokenizer
        /// </summary>
        protected static class Rules
        {
            /// <summary>
            /// StringLiteral = ~('=' | ':' | '\'');
            /// </summary>
            public static bool StringLiteral(Tokenizer data)
            {
                int count; for (count = 0; !data.EndOfStream; count++)
                {
                    Char32 c = data.PeekCharacter();
                    switch (c)
                    {
                        case '=':
                        case ':':
                        case '\'': return (count > 0);
                        default:
                            {
                                data.Position++;
                            }
                            break;
                    }
                }
                return (count > 0);
            }

            /// <summary>
            /// QuotedStringLiteral = '\'' ('\\'' | ~'\'')+ '\'';
            /// </summary>
            public static bool QuotedStringLiteral(Tokenizer data)
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
                    }
                    skip = !skip && (c == '\\');
                    data.Position++;
                }
                return false;
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
        }

        /// <summary>
        /// Determines if a character is a valid numeric to this tokenizer
        /// </summary>
        public static bool IsNumericChar(Char32 c)
        {
            return ((c >= '0' && c <= '9') | c == 'x' || c == 'X' || c == 'b' || c == 'B');
        }
    }
}
