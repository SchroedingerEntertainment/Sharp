// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// A generic parsed data token obtained from a provider
    /// </summary>
    public struct PropertyToken
    {
        bool isValue;
        /// <summary>
        /// Determines if this token is a value or a property
        /// </summary>
        public bool IsValue
        {
            get { return isValue; }
        }

        object value;
        /// <summary>
        /// Either a value or name object
        /// </summary>
        public object Value
        {
            get { return value; }
        }

        /// <summary>
        /// Creates a new generic data token
        /// </summary>
        public PropertyToken(bool isValue, object value)
        {
            this.isValue = isValue;
            this.value = value;
        }
    }
}