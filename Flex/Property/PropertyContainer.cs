// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SE.Flex
{
    /// <summary>
    /// Contains property data of a specific property ID and type
    /// </summary>
    public class PropertyContainer : IEnumerable<KeyValuePair<UInt32, object>>
    {
        readonly SortedDictionary<UInt32, object> items;
        ReadWriteLock itemLock;

        readonly Type type;
        /// <summary>
        /// The specified data type of property data contained
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in this container
        /// </summary>
        public int Count
        {
            get 
            {
                itemLock.ReadLock();
                try
                {
                    return items.Count;
                }
                finally
                {
                    itemLock.ReadRelease();
                }
            }
        }

        /// <summary>
        /// Initializes a new container instance of the given base type
        /// </summary>
        public PropertyContainer(Type type)
        {
            this.items = new SortedDictionary<UInt32, object>();
            this.itemLock = new ReadWriteLock();
            this.type = type;
        }

        /// <summary>
        /// Try to acquire inclusive read access to the items
        /// </summary>
        public void ReadLock()
        {
            itemLock.ReadLock();
        }
        /// <summary>
        /// Try to acquire inclusive read access to the items
        /// </summary>
        public Task ReadLockAsync()
        {
            return itemLock.ReadLockAsync();
        }
        /// <summary>
        /// Passes access back
        /// </summary>
        public void ReadRelease()
        {
            itemLock.ReadRelease();
        }

        /// <summary>
        /// Try to acquire exclusive write access to the items
        /// </summary>
        public void WriteLock()
        {
            itemLock.WriteLock();
        }
        /// <summary>
        /// Try to acquire exclusive write access to the items
        /// </summary>
        public Task WriteLockAsync()
        {
            return itemLock.WriteLockAsync();
        }
        /// <summary>
        /// Passes access back
        /// </summary>
        public void WriteRelease()
        {
            itemLock.WriteRelease();
        }

        /// <summary>
        /// Sets the specified entity's property value
        /// </summary>
        /// <param name="objectId">The id of the entity to change</param>
        /// <param name="value">The value of the property to set</param>
        /// <returns>True if the property of the specified entity was set successfully, false otherwise.</returns>
        public bool Set(TemplateId objectId, object value)
        {
            if (value.GetType() == type)
            {
                UInt32 id = objectId.ObjectId;
                itemLock.WriteLock();
                try
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, value);
                    }
                    else items[id] = value;
                }
                finally
                {
                    itemLock.WriteRelease();
                }
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Determines whether this container contains a property of the specified entity
        /// </summary>
        /// <param name="objectId">The id of the entity to lookup</param>
        /// <returns>True if the container contains a property of the specified entity, false otherwise</returns>
        public bool Contains(TemplateId objectId)
        {
            UInt32 id = objectId.ObjectId;
            itemLock.ReadLock();
            try
            {
                return items.ContainsKey(id);
            }
            finally
            {
                itemLock.ReadRelease();
            }
        }

        /// <summary>
        /// Gets the property value associated with the specified entity
        /// </summary>
        /// <param name="objectId">The id of the entity to lookup</param>
        /// <param name="value">
        /// When this method returns, property contains the value associated with the specified entity, 
        /// if the entity is found, or else the default value for the type of the value parameter otherwise. 
        /// This parameter is passed uninitialized
        /// </param>
        /// <returns>True if the container contains a property of the specified entity, false otherwise</returns>
        public bool TryGet(TemplateId objectId, out object value)
        {
            UInt32 id = objectId.ObjectId;
            itemLock.ReadLock();
            try
            {
                return items.TryGetValue(id, out value);
            }
            finally
            {
                itemLock.ReadRelease();
            }
        }

        /// <summary>
        /// Removes the entity's property value from this container
        /// </summary>
        /// <param name="objectId">The id of the entity to remove</param>
        /// <returns>
        /// True if the property of the specified entity is successfully found and removed, false otherwise. 
        /// This method returns false if the entity is not found in this container
        /// </returns>
        public bool Remove(TemplateId objectId)
        {
            UInt32 id = objectId.ObjectId;
            itemLock.WriteLock();
            try
            {
                return items.Remove(id);
            }
            finally
            {
                itemLock.WriteRelease();
            }
        }

        public IEnumerator<KeyValuePair<UInt32, object>> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
