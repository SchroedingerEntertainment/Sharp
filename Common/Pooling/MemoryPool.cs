// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace System.Collections.Generic
{
    /// <summary>
    /// Provides caching behavior for frequently reused MemoryStream objects
    /// </summary>
    public static class MemoryPool<T> where T : MemoryStream, new()
    {
        private readonly static ObjectPool<T> pool;

        static MemoryPool()
        {
            pool = new ObjectPool<T>(MemoryPolicy<T>.Default);
        }

        /// <summary>
        /// Gets the next available object instance from the cache
        /// </summary>
        /// <returns>A cached object instance if available, a newly created one otherwise</returns>
        public static T Get()
        {
            return pool.Get();
        }

        /// <summary>
        /// Returns the provided object instance into the cache. The instance will be reset to
        /// default and discarded if capacity is exceeded
        /// </summary>
        /// <param name="instance">An object instance to cache for reuse</param>
        /// <returns>True if the instance is cached properly, false otherwise</returns>
        public static bool Return(T instance)
        {
            return pool.Return(instance);
        }
    }
}