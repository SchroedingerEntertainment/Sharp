// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Converts a given string to title case
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (str.Length > 1) return Char.ToUpperInvariant(str[0]) + str.Substring(1);
            else return Char.ToUpperInvariant(str[0]).ToString();
        }
        /// <summary>
        /// Converts a given string to camel case
        /// </summary>
        public static string ToCamelCase(this string str)
        {
            if (str.Length > 1) return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            else return Char.ToLowerInvariant(str[0]).ToString();
        }
    }
}