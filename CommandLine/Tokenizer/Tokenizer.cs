// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE.Parsing;

namespace SE.CommandLine
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Tokenizer : StreamTokenizer<Token, TokenizerState>
    {
        /// <summary>
        /// Creates a new tokenizer instance
        /// </summary>
        public Tokenizer(Stream stream, bool isUtf8)
            : base(stream, isUtf8)
        { }

        /// <summary>
        /// Preprocessor directives
        /// </summary>
        protected override Token GetToken(object context)
        {
            switch (GetCharacter())
            {
                #region ResponseFile = '@';
                case '@':
                    {
                        return Token.ResponseFile;
                    }
                #endregion

                #region Separator = ('=' | ':');
                case '=':
                case ':':
                    {
                        return Token.Separator;
                    }
                #endregion

                #region QuotedStringLiteral = '\'' ('\\'' | ~'\'')+ '\'';
                case '\'':
                    {
                        RawDataBuffer.Discard(1);
                        Rules.QuotedStringLiteral(this);
                        if (RawDataBuffer.Head == '\'')
                        {
                            RawDataBuffer.Discard(1);
                        }
                        return Token.QuotedStringLiteral;
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

                #region StringLiteral = ~('=' | ':' | '\'');
                default:
                    {
                        Rules.StringLiteral(this);
                        return Token.StringLiteral;
                    }
                #endregion

                case '-': switch(PeekCharacter())
                    {
                        #region DoubleDash (--)
                        case '-':
                            {
                                RawDataBuffer.Position++;
                                if (PeekCharacter() != 0)
                                {
                                    return Token.LongIdentifier;
                                }
                                else return Token.Delimiter;
                            }
                        #endregion

                        #region Dash (-)
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
                        case '?':
                            {
                                return Token.Identifier;
                            }
                        #endregion
                    }
                    goto Numeric;

                #region Slash (/)
                case '/':
                    {
                        return Token.Identifier;
                    }
                #endregion
            }
        }
    }
}
