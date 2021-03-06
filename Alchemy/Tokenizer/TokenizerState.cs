﻿// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Alchemy
{
    /// <summary>
    /// Defines the states of the tokenizer
    /// </summary>
    public enum TokenizerState : byte
    {
        Initial = 0,

        /// <summary>
        /// The first token of a line
        /// </summary>
        AfterWhitespace,
    }
}