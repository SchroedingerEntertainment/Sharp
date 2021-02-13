// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using SE.Parsing;

namespace SE.Alchemy
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
        {
            this.newLineCharacter = (stream.Position == 0);
        }

        /// <summary>
        /// Preprocessor directives
        /// </summary>
        protected override Token GetToken(object context)
        {
            Token result = ProcessGetToken();
            switch (result)
            {
                case Token.NewLine:
                    {
                        State.Set(TokenizerState.Initial);
                    }
                    break;
                case Token.Comment:
                case Token.Whitespace:
                    break;
                default:
                    {
                        if (State.Current == TokenizerState.Initial)
                            State.Set(TokenizerState.AfterWhitespace);
                    }
                    break;
            }
            return result;
        }

        Token ProcessGetToken()
        {
            switch (PeekCharacter())
            {
                case '#': if(State.Current == TokenizerState.Initial)
                    {
                        RawDataBuffer.Position++;
                        BaseRules.NullLiteral(this);
                        switch (PeekCharacter())
                        {
                            case 'i':
                                {
                                    RawDataBuffer.Position++;
                                    switch (PeekCharacter())
                                    {
                                        case 'f':
                                            {
                                                RawDataBuffer.Position++;
                                                Char32 c = PeekCharacter();
                                                switch (c)
                                                {
                                                    #region #ifdef
                                                    case 'd':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.IfdefDirective(this))
                                                                return Token.IfdefDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #ifndef
                                                    case 'n':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.IfndefDirective(this))
                                                                return Token.IfndefDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #if
                                                    default:
                                                        {
                                                            if (!IsIdentifierChar(c))
                                                                return Token.IfDirective;
                                                        }
                                                        break;
                                                        #endregion
                                                }
                                            }
                                            break;

                                        #region #import
                                        case 'm':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.ImportDirective(this))
                                                    return Token.ImportDirective;
                                            }
                                            break;
                                        #endregion
                                    }
                                    RawDataBuffer.Position = 1;
                                }
                                goto default;

                            case 'e':
                                {
                                    RawDataBuffer.Position++;
                                    switch (PeekCharacter())
                                    {
                                        case 'l':
                                            {
                                                RawDataBuffer.Position++;
                                                switch (PeekCharacter())
                                                {
                                                    #region #elif
                                                    case 'i':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.ElifDirective(this))
                                                                return Token.ElifDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #else
                                                    case 's':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.ElseDirective(this))
                                                                return Token.ElseDirective;
                                                        }
                                                        break;
                                                    #endregion
                                                }
                                            }
                                            break;

                                        #region #error
                                        case 'r':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.ErrorDirective(this))
                                                {
                                                    return Token.Error;
                                                }
                                            }
                                            break;
                                        #endregion

                                        case 'n':
                                            {
                                                RawDataBuffer.Position++;
                                                switch (PeekCharacter())
                                                {
                                                    #region #endif
                                                    case 'd':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.EndifDirective(this))
                                                                return Token.EndifDirective;
                                                        }
                                                        break;
                                                    #endregion

                                                    #region #enable
                                                    case 'a':
                                                        {
                                                            RawDataBuffer.Position++;
                                                            if (Rules.EnableDirective(this))
                                                                return Token.EnableDirective;
                                                        }
                                                        break;
                                                    #endregion
                                                }
                                            }
                                            break;
                                    }
                                    RawDataBuffer.Position = 1;
                                }
                                goto default;

                            
                            case 'd':
                                {
                                    RawDataBuffer.Position++;
                                    switch (PeekCharacter())
                                    {
                                        #region #define
                                        case 'e':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.DefineDirective(this))
                                                    return Token.DefineDirective;
                                            }
                                            break;
                                        #endregion

                                        #region #disable
                                        case 'i':
                                            {
                                                RawDataBuffer.Position++;
                                                if (Rules.DisableDirective(this))
                                                    return Token.DisableDirective;
                                            }
                                            break;
                                        #endregion
                                    }
                                }
                                goto default;
                            

                            #region #undef
                            case 'u':
                                {
                                    RawDataBuffer.Position++;
                                    if (Rules.UndefDirective(this))
                                    {
                                        return Token.UndefDirective;
                                    }
                                    else RawDataBuffer.Position = 1;
                                }
                                goto default;
                            #endregion

                            #region #warn
                            case 'w':
                                {
                                    RawDataBuffer.Position++;
                                    if (Rules.WarnDirective(this))
                                    {
                                        return Token.Warning;
                                    }
                                    else RawDataBuffer.Position = 1;
                                }
                                goto default;
                            #endregion

                            #region SingleLineComment = '#'(~'\\\n') *;
                            default:
                                {
                                    if (Rules.SingleLineComment(this))
                                        return Token.Comment;
                                }
                                break;
                            #endregion
                        }
                    }
                    goto default;

                #region Anything else
                default:
                    {
                        return ReadGenericTokens();
                    }
                #endregion
            }
        }
    }
}
