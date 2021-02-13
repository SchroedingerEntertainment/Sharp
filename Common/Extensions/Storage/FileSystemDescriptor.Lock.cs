// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace System.IO
{
    public static partial class FileSystemDescriptorExtension
    {
        /// <summary>
        /// Obtains the mutually exclusive lock that belongs to this file system object
        /// </summary>
        public static NamedSpinlock GetExclsuiveLock(this FileSystemDescriptor fsd)
        {
            return new NamedSpinlock(fsd.GetAbsolutePath());
        }

        /// <summary>
        /// Obtains the multy read single write lock that belongs to this file system object
        /// </summary>
        public static NamedReadWriteLock GetLock(this FileSystemDescriptor fsd)
        {
            return new NamedReadWriteLock(fsd.GetAbsolutePath());
        }
    }
}