// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace System.Collections.Generic
{
    /// <summary>
    /// Default memory policy
    /// </summary>
    public class MemoryPolicy<T> : IPoolingPolicy<T> where T : MemoryStream, new()
    {
        /// <summary>
        /// Defines a threshold up to which streams are stored
        /// </summary>
        public const int AutoDisposeSize = 65536; //bytes

        /// <summary>
        /// A default memory pooling policy instance
        /// </summary>
        public readonly static MemoryPolicy<T> Default = new MemoryPolicy<T>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public MemoryPolicy()
        { }

        public T Create()
        {
            return new T();
        }
        public bool Return(T instance)
        {
            if (instance.Length < AutoDisposeSize)
            {
                instance.Clear();
                return true;
            }
            else return false;
        }
        public bool Delete(T instance)
        {
            instance.Dispose();
            return true;
        }
    }
}