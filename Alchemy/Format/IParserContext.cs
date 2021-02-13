// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Alchemy
{
    /// <summary>
    /// Defines a context target passed to the text parser
    /// </summary>
    public interface IParserContext
    {
        /// <summary>
        /// Requests adding a new file format module into this context
        /// </summary>
        /// <param name="id">The name of the module to be added</param>
        bool AddModule(string id);

        /// <summary>
        /// Tries to resolve an import request from the provided path
        /// </summary>
        /// <param name="path">The reauested path to be completed by the underlaying handler</param>
        /// <param name="prefix">A prefix to set before the file content</param>
        /// <param name="stream">The text data stream to integrate</param>
        /// <returns>True if the stream was successfully created, false otherwise</returns>
        bool ResolveFileReference(object context, ref string path, ref string prefix, out Stream stream);

        /// <summary>
        /// Iterates through the added format modules and transforms the 
        /// provided token data if needed
        /// </summary>
        string Transform(Token token, string input);
    }
}
