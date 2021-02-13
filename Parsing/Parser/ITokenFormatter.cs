// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// An interface to a stream formatter used
    /// </summary>
    public interface ITokenFormatter : IDisposable
    {
        /// <summary>
        /// Determines if the underlying stream has reached it's end
        /// </summary>
        bool EndOfStream { get; }

        /// <summary>
        /// Tries to get the next token from the stream
        /// </summary>
        /// <returns>True if successfull, false otherwise</returns>
        bool ParseNext();
    }
}