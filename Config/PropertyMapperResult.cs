// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Contains the result of a mapping operation
    /// </summary>
    public struct PropertyMapperResult : IDisposable
    {
        Dictionary<string, int> parsed;
        /// <summary>
        /// Contains successfully parsed and assigned data properties
        /// </summary>
        public Dictionary<string, int> Parsed
        {
            get { return parsed; }
            internal set { parsed = value; }
        }

        HashSet<string> skipped;
        /// <summary>
        /// Contains field and class property names which haven't been assigned. This
        /// can be caused by a data property not existing or of wrong data type
        /// </summary>
        public HashSet<string> Skipped
        {
            get { return skipped; }
            internal set { skipped = value; }
        }

        int verbs;
        /// <summary>
        /// The number of assigned data values from a verb argument match
        /// </summary>
        public int Verbs
        {
            get { return verbs; }
            internal set { verbs = value; }
        }

        List<Exception> errors;
        /// <summary>
        /// Contains errors occured while mapping the data properties
        /// </summary>
        public List<Exception> Errors
        {
            get { return errors; }
            internal set { errors = value; }
        }

        public void Dispose()
        {
            if (parsed != null)
            {
                CollectionPool<Dictionary<string, int>, string, int>.Return(parsed);
                parsed = null;
            }
            if (skipped != null)
            {
                CollectionPool<HashSet<string>, string>.Return(skipped);
                skipped = null;
            }
            if (errors != null)
            {
                CollectionPool<List<Exception>, Exception>.Return(errors);
                errors = null;
            }
        }
    }
}
