// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ObjectExtension
    {
        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public static string ToStringNoExcept(this object instance)
        {
            if (instance != null)
            {
                return instance.ToString();
            }
            else return "null";
        }
    }
}