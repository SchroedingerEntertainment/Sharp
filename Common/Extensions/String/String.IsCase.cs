// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class StringExtension
    {
        /// <summary>
        /// Determines if this string is upper case only
        /// </summary>
        public static bool IsUpperCase(this string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsUpper(str[i]))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Determines if this string is lower case only
        /// </summary>
        public static bool IsLowerCase(this string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsUpper(str[i]))
                    return false;
            }
            return true;
        }
    }
}