// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Marks a property or field as configurable by value index
    /// </summary>
    public class VerbPropertyAttribute : PropertyAttribute
    {
        int index;
        /// <summary>
        /// The index of the unbound value to assign to this field or property
        /// </summary>
        public int Index
        {
            get { return index; } 
        }

        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="index">A zero based index of an item in the unbound value collection</param>
        public VerbPropertyAttribute(int index)
        {
            this.index = index;
        }
    }
}
