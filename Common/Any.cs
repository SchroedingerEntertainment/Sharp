// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Defines a transient data type that must not be set to anything except the
    /// default value
    /// </summary>
    public struct Any<T>
    {
        /// <summary>
        /// A predefined empty transient
        /// </summary>
        public readonly static Any<T> Empty = new Any<T>();

        readonly T value;
        /// <summary>
        /// Tries to access a value stored in this transient
        /// </summary>
        public T Value
        {
            get
            {
                if (!hasValue)
                {
                    throw new InvalidOperationException();
                }
                return value;
            }
        }

        readonly bool hasValue;
        /// <summary>
        /// Determines if a value is stored in this transient
        /// </summary>
        public bool HasValue
        {
            get { return hasValue; }
        }

        /// <summary>
        /// Initializes a new container that already contains a type
        /// </summary>
        /// <param name="value"></param>
        public Any(T value)
        {
            this.value = value;
            this.hasValue = true;
        }

        public static implicit operator Any<T>(T value)
        {
            return new Any<T>(value);
        }
        public static implicit operator bool(Any<T> value)
        {
            return value.hasValue;
        }
        public static explicit operator T(Any<T> value)
        {
            return value.value;
        }

        /// <summary>
        /// Tries to obtain a value stored in this transient
        /// </summary>
        /// <returns>True if a value is contained, false otherwise</returns>
        public bool TryGetValue(out T value)
        {
            value = Get();
            return hasValue;
        }

        /// <summary>
        /// Tries to obtain a value stored in this transient or default
        /// </summary>
        /// <param name="default">A default value that shall be returned if no value is present</param>
        public T Get(T @default = default(T))
        {
            if (!hasValue)
            {
                return @default;
            }
            else return value;
        }

        public override bool Equals(object other)
        {
            if (!hasValue)
            {
                return other == null;
            }
            else if (other == null)
            {
                return false;
            }
            else return value.Equals(other);
        }
        public override int GetHashCode()
        {
            return ((hasValue) ? value.GetHashCode() : 0);
        }
        public override string ToString()
        {
            return ((hasValue) ? value.ToString() : string.Empty);
        }
    }
}