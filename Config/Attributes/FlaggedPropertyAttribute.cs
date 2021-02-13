// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Marks a property or field as configurable by flags
    /// </summary>
    public class FlaggedPropertyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="enumType">The type of an Enum the flags should load from </param>
        public FlaggedPropertyAttribute()
        { }
    }
}
