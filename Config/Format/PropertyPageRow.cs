// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// A generic text-line in a PropertyPage
    /// </summary>
    public class PropertyPageRow : IDisposable
    {
        List<string> keys;
        /// <summary>
        /// A set of keys put into the left-most column
        /// </summary>
        public List<string> Keys
        {
            get { return keys; }
        }

        /// <summary>
        /// Creates a new row instance
        /// </summary>
        public PropertyPageRow()
        {
            this.keys = CollectionPool<List<string>, string>.Get();
        }
        public void Dispose()
        {
            if (keys != null)
            {
                CollectionPool<List<string>, string>.Return(keys);
                keys = null;
            }
        }
    }
}
