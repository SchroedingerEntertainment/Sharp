// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Runtime
{
    /// <summary>
    /// An interface for any logging operation
    /// </summary>
    public interface ILogSystem
    {
        /// <summary>
        /// Puts a message to the LOG channel
        /// </summary>
        void Log(string message);
        /// <summary>
        /// Puts a message to the WARN channel
        /// </summary>
        void Warning(string message);
        /// <summary>
        /// Puts a message to the ERROR channel
        /// </summary>
        void Error(string message);
    }
}
