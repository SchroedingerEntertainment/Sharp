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
    public abstract class PropertyAttribute : Attribute
    {
        string cluster;
        /// <summary>
        /// A cluster ID this property is grouped to
        /// </summary>
        public string Cluster
        {
            get { return cluster; }
            set { cluster = value; }
        }

        object defaultValue;
        /// <summary>
        /// A defined default value if the parameter isn't declared or doesn't match the
        /// type of the decorated member
        /// </summary>
        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        Type typeConverter;
        /// <summary>
        /// Sets a custom converter to apply to field or property assigned values
        /// </summary>
        public Type TypeConverter
        {
            get { return typeConverter; }
            set { typeConverter = value; }
        }

        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        /// <param name="defaultValue">A value to be set if the parameter wasn't defined or doesn't match the destination type</param>
        public PropertyAttribute(object defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        /// <summary>
        /// Declares this field or property as configurable
        /// </summary>
        public PropertyAttribute()
        { }
    }
}
