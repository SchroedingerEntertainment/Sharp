// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using SE.Parsing;

namespace SE.CppLang
{
    public static partial class ParserTokenExtension
    {
        /// <summary>
        /// Transforms this token into a formatted value string
        /// </summary>
        public static string Format(this ParserToken<Token> token)
        {
            switch (token.Type)
            {
                case Token.CharacterLiteral:
                    {
                        return String.Concat("\'", token.Buffer, "\'");
                    }
                case Token.StringLiteral:
                    {
                        return String.Concat("\"", token.Buffer, "\"");
                    }
                default:
                    {
                        return token.Buffer;
                    }
            }
        }
    }
}
