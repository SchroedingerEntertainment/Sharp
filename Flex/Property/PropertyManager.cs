// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE.Flex
{
    /// <summary>
    /// Handles entity to component relations
    /// </summary>
    public sealed class PropertyManager
    {
        private readonly static Dictionary<UInt32, PropertyContainer> properties;
        private static ReadWriteLock propertyLock;

        static PropertyManager()
        {
            properties = new Dictionary<UInt32, PropertyContainer>();
            propertyLock = new ReadWriteLock();
        }

        /// <summary>
        /// Sets a new property value to the given flex object or changes the property type already
        /// present at the object
        /// </summary>
        /// <param name="propertyId">The object ID used to identify the object</param>
        /// <param name="value">The property value to set</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetProperty(TemplateId propertyId, object value)
        {
            UInt32 id = propertyId.ComponentId;
            PropertyContainer container;
            propertyLock.ReadLock();
            try
            {
                properties.TryGetValue(id, out container);
            }
            finally
            {
                propertyLock.ReadRelease();
            }
            if (container == null)
            {
                propertyLock.WriteLock();
                try
                {
                    if (!properties.TryGetValue(id, out container))
                    {
                        container = new PropertyContainer(value.GetType());
                        properties.Add(id, container);
                    }
                }
                finally
                {
                    propertyLock.WriteRelease();
                }
            }
            return container.Set(propertyId, value);
        }

        /// <summary>
        /// Tries to return all instances that match certain criteria
        /// </summary>
        /// <param name="predicate">A filter that will be applied to the lookup</param>
        /// <returns>The resulting list of property instances</returns>
        public static int FindProperties(Predicate<KeyValuePair<UInt32, object>> predicate, ICollection<object> result)
        {
            propertyLock.ReadLock();
            try
            {
                foreach (PropertyContainer container in properties.Values)
                {
                    container.ReadLock();
                    try
                    {
                        foreach (KeyValuePair<UInt32, object> item in container)
                            if (predicate(item))
                            {
                                result.Add(item.Value);
                            }
                    }
                    finally
                    {
                        container.ReadRelease();
                    }
                }
            }
            finally
            {
                propertyLock.ReadRelease();
            }
            return result.Count;
        }

        /// <summary>
        /// Tries to return all instances for a specific property
        /// </summary>
        /// <param name="propertyId">The property ID used to identify the property instances</param>
        /// <returns>True if property instances exist, false otherwise</returns>
        public static bool TryGetProperties(TemplateId propertyId, out PropertyContainer result)
        {
            UInt32 id = propertyId.ComponentId;
            propertyLock.ReadLock();
            try
            {
                if (!properties.TryGetValue(id, out result))
                    return false;
            }
            finally
            {
                propertyLock.ReadRelease();
            }
            return true;
        }
        /// <summary>
        /// Tries to return all instances for a specific property
        /// </summary>
        /// <param name="name">The property name used to identify the property instances</param>
        /// <returns>True if property instances exist, false otherwise</returns>
        public static bool TryGetProperties(string name, out PropertyContainer result)
        {
            return TryGetProperties(TemplateId.Invalid | name.Fnv32(), out result);
        }

        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <param name="propertyId">The property ID used to identify the object's property</param>
        /// <returns>True if the property instance exists on the object, false otherwise</returns>
        public static bool TryGetProperty(TemplateId propertyId, out object result)
        {
            PropertyContainer container; if (!TryGetProperties(propertyId, out container))
            {
                result = null;
                return false;
            }
            else return container.TryGet(propertyId, out result);
        }
        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">A property name to get the instance for</param>
        /// <returns>True if the property instance exists on the object, false otherwise</returns>
        public static bool TryGetProperty(TemplateId objectId, string name, out object result)
        {
            return TryGetProperty(objectId | name.Fnv32(), out result);
        }

        /// <summary>
        /// Clears all properties for the specific object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool Clear(TemplateId objectId)
        {
            bool result = false;
            List<UInt32> emptyProperties = CollectionPool<List<UInt32>, UInt32>.Get();
            try
            {
                propertyLock.ReadLock();
                try
                {
                    foreach (KeyValuePair<UInt32, PropertyContainer> container in properties)
                        if (container.Value.Remove(objectId))
                        {
                            if (!result)
                            {
                                result = true;
                            }
                            if (container.Value.Count == 0)
                            {
                                emptyProperties.Add(container.Key);
                            }
                        }
                }
                finally
                {
                    propertyLock.ReadRelease();
                }
                foreach (UInt32 id in emptyProperties)
                {
                    propertyLock.WriteLock();
                    try
                    {
                        PropertyContainer container; if (properties.TryGetValue(id, out container) && container.Count == 0)
                        {
                            properties.Remove(id);
                        }
                    }
                    finally
                    {
                        propertyLock.WriteRelease();
                    }
                }
            }
            finally
            {
                CollectionPool<List<UInt32>, UInt32>.Return(emptyProperties);
            }
            return result;
        }

        /// <summary>
        /// Removes an existing property from the given flex object
        /// </summary>
        /// <param name="objectId">The object ID used to identify the object</param>
        /// <param name="name">A property name to remove if existing</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RemoveProperty(TemplateId propertyId)
        {
            UInt32 id = propertyId.ComponentId;
            PropertyContainer container;
            propertyLock.ReadLock();
            try
            {
                if (!properties.TryGetValue(id, out container))
                    return false;
            }
            finally
            {
                propertyLock.ReadRelease();
            }
            if (container.Remove(propertyId))
            {
                propertyLock.WriteLock();
                try
                {
                    properties.Remove(id);
                }
                finally
                {
                    propertyLock.WriteRelease();
                }
                return true;
            }
            else return false;
        }
    }
}
