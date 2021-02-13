// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// A token created from the tree-builder. The tree-builder source stream consists of
    /// such tokens
    /// </summary>
    public struct ParserToken<TokenId> where TokenId : struct, IConvertible, IComparable
    {
        TokenId type;
        /// <summary>
        /// The defined type of this token
        /// </summary>
        public TokenId Type
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
        public ParserToken(TokenId original, string buffer, TextPointer carret)
        {
            this.type = original;
            this.buffer = buffer;
            this.carret = carret;
        }
        
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(buffer))
            {
                return string.Concat(type, "(", buffer, ")");
            }
            else return type.ToString();
        }
    }
}
