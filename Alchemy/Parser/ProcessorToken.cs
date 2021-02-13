// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.Alchemy
{
    /// <summary>
    /// A token created from the preprocessor
    /// </summary>
    public struct TextProcessorToken
    {
        Token type;
        /// <summary>
        /// The defined type of this token
        /// </summary>
        public Token Type
        {
            get { return type; }
        }

        string buffer;
        /// <summary>
        /// A string buffer used by some token types
        /// </summary>
        public string Buffer
        {
            get { return buffer; }
        }

        TextPointer carret;
        /// <summary>
        /// The location in the source data this token has been
        /// detected at
        /// </summary>
        public TextPointer Carret
        {
            get { return carret; }
        }

        /// <summary>
        /// Creates a new token instance
        /// </summary>
        public TextProcessorToken(Token original, string buffer, TextPointer carret)
        {
            this.type = original;
            this.buffer = buffer;
            this.carret = carret;
        }

        public override string ToString()
        {
            switch (type)
            {
                #region Character
                case Token.Character:
                #endregion

                #region BogusDoubleQuotationLiteral
                case Token.BogusDoubleQuotationLiteral:
                #endregion

                #region BogusSingleQuotationLiteral
                case Token.BogusSingleQuotationLiteral:
                    {
                        return string.Concat(type.ToString(), "(", buffer, ")");
                    }
                #endregion

                #region Anything else
                default:
                    {
                        return Buffer.ToString();
                    }
                #endregion
            }
        }
    }
}
