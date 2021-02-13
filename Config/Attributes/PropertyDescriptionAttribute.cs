// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Marks a property or field as configurable
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PropertyDescriptionAttribute : Attribute
    {
        string text;
        /// <summary>
        /// A description text used when creating automated help pages
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        PropertyType type;
        /// <summary>
        /// Defines an informal type for this property
        /// </summary>
        public PropertyType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Adds an description to a data property field
        /// </summary>
        public PropertyDescriptionAttribute(string text)
        {
            this.text = text;
        }
    }
}
