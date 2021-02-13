// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE.Flex
{
    /// <summary>
    /// Handles entity to extension method relations
    /// </summary>
    #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
    public sealed class MethodManager : AppDomainʾ.ReferenceObject
    #else
    public static class MethodManager
    #endif
    {
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
        private static AppStatic<MethodManager> instance;

        readonly Dictionary<UInt64, Delegate> methods;
        ReadWriteLock methodLock;
        #else
        private readonly static Dictionary<UInt64, Delegate> methods;
        private static ReadWriteLock methodLock;
        #endif

        static MethodManager()
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
        {
            instance = new AppStatic<MethodManager>();
            instance.CreateValue();
        }
        public MethodManager()
        #endif
        {
            methods = new Dictionary<UInt64, Delegate>();
            methodLock = new ReadWriteLock();
        }

        /// <summary>
        /// Adds an extension method to the given flex object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">An extension name to add</param>
        /// <param name="extension">The extension delegate to add to the object</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AddMethod(TemplateId objectId, string name, Delegate extension)
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
        {
            return instance.Value.__AddMethod(objectId, name, extension);
        }

        bool __AddMethod(TemplateId objectId, string name, Delegate extension)
        #endif
        {
            objectId = (objectId | name.Fnv32());
            methodLock.WriteLock();
            try
            {
                if (!methods.ContainsKey(objectId))
                {
                    methods.Add(objectId, extension);
                }
                else return false;
            }
            finally
            {
                methodLock.WriteRelease();
            }
            return true;
        }

        /// <summary>
        /// Tries to return an extension method for a specific object
        /// </summary>
        /// <param name="methodId">The property ID used to identify the object's extension</param>
        /// <returns>True if the extension method exists on the object, false otherwise</returns>
        public static bool TryGetMethod(TemplateId methodId, out Delegate result)
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
        {
            return instance.Value.__TryGetMethod(methodId, out result);
        }

        bool __TryGetMethod(TemplateId methodId, out Delegate result)
        #endif
        {
            methodLock.ReadLock();
            try
            {
                return methods.TryGetValue(methodId, out result);
            }
            finally
            {
                methodLock.ReadRelease();
            }
        }
        /// <summary>
        /// Tries to return an extension method for a specific object
        /// </summary>
        /// <param name="objectId">The property ID used to identify the object's extension</param>
        /// <param name="name">An extension name to get a delegate for</param>
        /// <returns>True if the extension method exists on the object, false otherwise</returns>
        public static bool TryGetMethod(TemplateId objectId, string name, out Delegate result)
        {
            return TryGetMethod(objectId | name.Fnv32(), out result);
        }

        /// <summary>
        /// Clears all extension methods for the specific object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool Clear(TemplateId objectId)
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
        {
            return instance.Value.__Clear(objectId);
        }

        bool __Clear(TemplateId objectId)
        #endif
        {
            List<TemplateId> idList = CollectionPool<List<TemplateId>, TemplateId>.Get();
            try
            {
                methodLock.ReadLock();
                try
                {
                    foreach (TemplateId id in methods.Keys)
                        if (id.ObjectId == objectId.ObjectId)
                        {
                            idList.Add(id);
                        }
                }
                finally
                {
                    methodLock.ReadRelease();
                }
                foreach (TemplateId id in idList)
                {
                    methodLock.WriteLock();
                    try
                    {
                        methods.Remove(id);
                    }
                    finally
                    {
                        methodLock.WriteRelease();
                    }
                }
                return (idList.Count != 0);
            }
            finally
            {
                CollectionPool<List<TemplateId>, TemplateId>.Return(idList);
            }
        }

        /// <summary>
        /// Removes an existing extension method from the given flex object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">An extension name to remove if existing</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RemoveMethod(TemplateId objectId, string name)
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472 || net48
        {
            return instance.Value.__RemoveMethod(objectId, name);
        }

        bool __RemoveMethod(TemplateId objectId, string name)
        #endif
        {
            objectId = (objectId | name.Fnv32());
            methodLock.WriteLock();
            try
            {
                return methods.Remove(objectId);
            }
            finally
            {
                methodLock.WriteRelease();
            }
        }
    }
}
