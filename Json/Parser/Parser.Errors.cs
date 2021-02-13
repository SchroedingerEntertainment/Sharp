// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Json
{
    public partial class Parser
    {
        /// <summary>
        /// A collection of error messages that are used by the parser
        /// </summary>
        protected static class ErrorMessages
        {
            public const string InvalidStart = "({0}): Expected an Object or Array";
            public const string InvalidEnd = "({0}): Document not properly closed";

            public const string InvalidNumber = "({0}): Token '{1}' is not a number";
            public const string InvalidToken = "({0}): Invalid token '{1}', expected '{2}'";

            public const string ParserError = "({0}): Invalid token '{1}'";
        }
    }
}
