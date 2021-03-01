// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Config
{
    /// <summary>
    /// Converts string into FileDescriptor
    /// </summary>
    public class FileConverter : ITypeConverter
    {
        private readonly static Type FileType = typeof(FileDescriptor);

        /// <summary>
        /// Creates a new instance of this converter
        /// </summary>
        public FileConverter()
        { }

        public bool TryParseValue(Type targetType, object value, out object result)
        {
            if (targetType == FileType)
            {
                result = FileDescriptor.Create(value.ToString());
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
