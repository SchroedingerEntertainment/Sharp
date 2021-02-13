// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Marks a property or field as configurable by name
    /// </summary>
    public class NamedPropertyAttribute : PropertyAttribute
    {
        string[] id;
        /// <summary>
        /// An ID to match this field or property 
        /// </summary>
        public string[] Id
        {
            get { return id; } 
        }

        /// <summary>
        /// Declares this field or property as configurable by it's default name
        /// </summary>
        public NamedPropertyAttribute()
        { }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="long">A string code to indicate this field or property</param>
        public NamedPropertyAttribute(string @long)
        {
            this.id = new string[]
            {
                @long
            };
        }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="short">The single character code to indicate this field or property</param>
        public NamedPropertyAttribute(char @short)
            : this(new string(@short, 1))
        { }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="short">The single character code to indicate this field or property</param>
        /// <param name="long">A string code to indicate this field or property</param>
        public NamedPropertyAttribute(char @short, string @long)
        {
            this.id = new string[]
            {
                new string(@short, 1),
                @long
            };
        }
    }
}
