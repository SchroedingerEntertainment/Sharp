// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE.Parsing;

namespace SE.Json
{
    /// <summary>
    /// Parses JSON data into a stream of JSON tokens
    /// </summary>
    public partial class Tokenizer : StreamTokenizer<Token, TokenizerState>
    {
        public Tokenizer(Stream stream, bool isUtf8)
            : base(stream, isUtf8)
        { }

        protected override Token GetToken(object context)
        {
            switch (GetCharacter())
            {
                #region StringLiteral = '\"' ('\\"' | ~'\"')* '\"';
                case '\"':
                    {
                        if (Rules.StringLiteral(this))
                            return Token.String;
                    }
                    goto default;
                #endregion

                #region Numeric = (('-')?['0', '9']) ((('E' | 'e') ('+' | '-')) | ['0','9'] '.')*;
                case '-':
                    {
                        if (Rules.Numeric(this))
                            return Token.Numeric;
                    }
                    goto default;
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
                    {
                        Rules.Numeric(this);
                        return Token.Numeric;
                    }
                #endregion

                #region Boolean = 'true' | 'false';
                case 't':
                    {
                        if (Rules.TrueConstant(this))
                            return Token.True;
                    }
                    goto default;
                case 'f':
                    {
                        if (Rules.FalseConstant(this))
                            return Token.False;
                    }
                    goto default;
                #endregion

                #region Null = 'null';
                case 'n':
                    {
                        if (Rules.NullConstant(this))
                            return Token.Null;
                    }
                    goto default;
                #endregion

                #region BeginObject {
                case '{':
                    {
                        return Token.BeginObject;
                    }
                #endregion

                #region EndObject }
                case '}':
                    {
                        return Token.EndObject;
                    }
                #endregion

                #region BeginArray [
                case '[':
                    {
                        return Token.BeginArray;
                    }
                #endregion

                #region EndArray ]
                case ']':
                    {
                        return Token.EndArray;
                    }
                #endregion

                #region Colon (:)
                case ':':
                    {
                        return Token.Colon;
                    }
                #endregion

                #region Comma (,)
                case ',':
                    {
                        return Token.Comma;
                    }
                #endregion

                #region Whitespace;
                case '\r':
                case Char32.WhiteSpaceGroup.Space:
                case Char32.NewLineGroup.LineFeed:
                case Char32.WhiteSpaceGroup.HorizontalTab:
                    {
                        BaseRules.NullLiteral(this);
                        return Token.Whitespace;
                    }
                #endregion

                default: 
                    return Token.Invalid;
            }
        }
    }
}
