// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SE.Alchemy
{
    /// <summary>
    /// Defines a dynamic formatter module embedded into a property sheet
    /// </summary>
    public interface IFormatModule
    {
        /// <summary>
        /// A collection of error messages occured while loading
        /// </summary>
        IEnumerable<string> Errors { get; }

        /// <summary>
        /// Determines if the loader has encountered any errors
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Try to load the provided stream into the given data format module
        /// </summary>
        /// <param name="stream">The input text stream to load</param>
        /// <param name="encoding">The input data encoding</param>
        /// <returns>True if loaded successfully, false otherwise</returns>
        bool Load(Stream stream, Encoding encoding);

        /// <summary>
        /// Transforms the provided token data if needed
        /// </summary>
        string Transform(Token token, string input);
    }
}
