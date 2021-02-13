// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;

namespace SE.Config
{
    /// <summary>
    /// A key/value text-line in a PropertyPage
    /// </summary>
    public class PropertyPageKeyValueRow : PropertyPageRow
    {
        int verbIndex;
        /// <summary>
        /// A sort index to show this property at
        /// </summary>
        public int VerbIndex
        {
            get { return verbIndex; }
            internal set { verbIndex = value; }
        }

        PropertyType type;
        /// <summary>
        /// The type of this property
        /// </summary>
        public PropertyType Type
        {
            get { return type; }
            internal set { type = value; }
        }

        string defaultValue;
        /// <summary>
        /// An optional default value
        /// </summary>
        public string DefaultValue
        {
            get { return defaultValue; }
            internal set { defaultValue = value; }
        }

        string description;
        /// <summary>
        /// A description text written into the right most column
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// The common width of the left most column this line needs
        /// </summary>
        public int Width
        {
            get { return Keys.Max(x => x.Length); }
        }

        /// <summary>
        /// Creates a new row instance with a description
        /// </summary>
        public PropertyPageKeyValueRow(string description)
        {
            this.description = description;
        }
        /// <summary>
        /// Creates a new row instance with a description
        /// </summary>
        public PropertyPageKeyValueRow(string description, PropertyType type)
        {
            this.description = description;
            this.type = type;
        }
    }
}
