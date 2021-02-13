// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    /// <summary>
    /// Default generic object policy
    /// </summary>
    public class GenericObjectPolicy<TValue> : IPoolingPolicy<TValue> where TValue : class, new()
    {
        /// <summary>
        /// A default generic pooling policy instance
        /// </summary>
        public readonly static GenericObjectPolicy<TValue> Default = new GenericObjectPolicy<TValue>();

        /// <summary>
        /// Creates a new pooling policy instance
        /// </summary>
        public GenericObjectPolicy()
        { }

        public TValue Create()
        {
            return new TValue();
        }
        public bool Return(TValue instance)
        {
            return true;
        }
        public bool Delete(TValue instance)
        {
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            return true;
        }
    }
}