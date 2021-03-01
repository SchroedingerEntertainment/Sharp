// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Config
{
    /// <summary>
    /// Converts string into PathDescriptor
    /// </summary>
    public class PathConverter : ITypeConverter
    {
        private readonly static Type PathType = typeof(PathDescriptor);

        /// <summary>
        /// Creates a new instance of this converter
        /// </summary>
        public PathConverter()
        { }

        public bool TryParseValue(Type targetType, object value, out object result)
        {
            if (targetType == PathType)
            {
                result = new PathDescriptor(value.ToString());
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
