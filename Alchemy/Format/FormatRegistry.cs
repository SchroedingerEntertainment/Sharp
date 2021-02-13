// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace SE.Alchemy
{
    /// <summary>
    /// A registry managing file format modules
    /// </summary>
    public static class FormatRegistry
    {
        class LowerCaseStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLowerInvariant().GetHashCode();
            }
        }
        private static readonly Type FormatModuleType = typeof(IFormatModule);

        private readonly static Dictionary<string, Func<IFormatModule>> modules;
        private static ReadWriteLock moduleLock;

        /// <summary>
        /// The amount of modules in the registry
        /// </summary>
        public static int Count
        {
            get { return modules.Count; }
        }

        static FormatRegistry()
        {
            modules = new Dictionary<string, Func<IFormatModule>>(new LowerCaseStringComparer());
            moduleLock = new ReadWriteLock();
        }

        /// <summary>
        /// Adds a module creator to the registry
        /// </summary>
        /// <returns>True if the module was added successfully, false otherwise</returns>
        public static bool Add(string id, Func<IFormatModule> creator)
        {
            moduleLock.WriteLock();
            try
            {
                if (!modules.ContainsKey(id))
                {
                    modules.Add(id, creator);
                    return true;
                }
                else return false;
            }
            finally
            {
                moduleLock.WriteRelease();
            }
        }

        /// <summary>
        /// Adds a module type to the registry
        /// </summary>
        /// <returns>True if the module was added successfully, false otherwise</returns>
        public static bool Add(string id, Type type)
        {
            if (FormatModuleType.IsAssignableFrom(type))
            {
                return Add(id, type.GetCreator<Func<IFormatModule>>());
            }
            else throw new ArgumentOutOfRangeException("type");
        }
        /// <summary>
        /// Adds the module type to the registry
        /// </summary>
        /// <returns>True if the module was added successfully, false otherwise</returns>
        public static bool Add<T>(string id) where T : IFormatModule
        {
            return Add(id, typeof(T));
        }

        /// <summary>
        /// Determines if certain format module ID is present in the registry
        /// </summary>
        /// <returns>Treu if the format module ID is known, false othwerwise</returns>
        public static bool Contains(string id)
        {
            moduleLock.ReadLock();
            try
            {
                return modules.ContainsKey(id);
            }
            finally
            {
                moduleLock.ReadRelease();
            }
        }
        
        /// <summary>
        /// Removes all entries from the registry
        /// </summary>
        public static void Clear()
        {
            modules.Clear();
        }

        /// <summary>
        /// Tries to obtain a file format module instance from the registry
        /// </summary>
        /// <returns>Treu if the format module ID is known, false othwerwise</returns>
        public static bool TryGetModule(string id, out IFormatModule module)
        {
            Func<IFormatModule> creator; if (modules.TryGetValue(id, out creator))
            {
                module = creator();
                return true;
            }
            else
            {
                module = null;
                return false;
            }
        }

        /// <summary>
        /// Removes a single entry from the registry
        /// </summary>
        public static bool Remove(string id)
        {
            return modules.Remove(id);
        }
    }
}
