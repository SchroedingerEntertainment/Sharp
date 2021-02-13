// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Represents an interconnected object instance between two end points
    /// </summary>
    public class FinalizerObject : IDisposable
    {
        bool disposed;
        /// <summary>
        /// Determines if this object has already been disposed
        /// </summary>
        public bool Disposed
        {
            get { return disposed; }
        }

        /// <summary>
        /// Creates a new interconnected object instance
        /// </summary>
        public FinalizerObject()
        { }
        ~FinalizerObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }
    }
}