// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace SE.Flex
{
    /// <summary>
    /// Base object for a flex entity
    /// </summary>
    public class FlexObject : IFlexObject, IDisposable
    {
        TemplateId template;
        public TemplateId Template
        {
            get { return template; }
        }

        /// <summary>
        /// Instantiates a flex object from the provided template
        /// </summary>
        /// <param name="template">A template ID this flex object will be bound to</param>
        public FlexObject(TemplateId template)
        {
            this.template = template;
        }
        /// <summary>
        /// Instantiates a new flex object
        /// </summary>
        public FlexObject()
         : this(TemplateId.Create())
        { }
        public virtual void Dispose()
        {
            PropertyManager.Clear(template);
            MethodManager.Clear(template);
        }

        /// <summary>
        /// Sets a new property value to the given flex object or changes the property type already
        /// present at the object
        /// </summary>
        /// <param name="propertyId">The object ID used to identify the object</param>
        /// <param name="value">The property value to set</param>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool SetProperty<T>(TemplateId propertyId, T value)
        {
            return PropertyManager.SetProperty(propertyId, value);
        }
        /// <summary>
        /// Sets a new property value to the given flex object or changes the property type already
        /// present at the object
        /// </summary>
        /// <param name="name">The property name to modify</param>
        /// <param name="value">The property value to set</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetProperty<T>(string name, T value)
        {
            return SetProperty<T>(template | name.Fnv32(), value);
        }
        /// <summary>
        /// Sets a new property value to the given flex object or changes the property type already
        /// present at the object
        /// </summary>
        /// <param name="value">The property value to set</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetProperty<T>(T value)
        {
            return SetProperty<T>(template | typeof(T).FullName.Fnv32(), value);
        }

        /// <summary>
        /// Determines if the given property exists
        /// </summary>
        /// <param name="propertyId">The object ID used to identify the object</param>
        /// <returns>True if the property exists, false otherwise</returns>
        public virtual bool HasProperty(TemplateId propertyId)
        {
            object _unused;
            return PropertyManager.TryGetProperty(propertyId, out _unused);
        }
        /// <summary>
        /// Determines if the given property exists
        /// </summary>
        /// <param name="name">The property name to find</param>
        /// <returns>True if the property exists, false otherwise</returns>
        public bool HasProperty(string name)
        {
            return HasProperty(template | name.Fnv32());
        }
        /// <summary>
        /// Determines if the given property exists
        /// </summary>
        /// <returns>True if the property exists, false otherwise</returns>
        public bool HasProperty<T>()
        {
            return HasProperty(template | typeof(T).FullName.Fnv32());
        }

        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <param name="propertyId">The object ID used to identify the object</param>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool TryGetProperty<T>(TemplateId propertyId, out T result)
        {
            object value; if (PropertyManager.TryGetProperty(propertyId, out value) && value is T)
            {
                result = (T)value;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }
        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <param name="name">The property name to return</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TryGetProperty<T>(string name, out T result)
        {
            return TryGetProperty<T>(template | name.Fnv32(), out result);
        }
        /// <summary>
        /// Tries to return a property instance for a specific object
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool TryGetProperty<T>(out T result)
        {
            return TryGetProperty<T>(template | typeof(T).FullName.Fnv32(), out result);
        }

        /// <summary>
        /// Removes an existing property from the given flex object
        /// </summary>
        /// <param name="propertyId">The object ID used to identify the object</param>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool RemoveProperty<T>(TemplateId propertyId)
        {
            return PropertyManager.RemoveProperty(propertyId);
        }
        /// <summary>
        /// Removes an existing property from the given flex object
        /// </summary>
        /// <param name="name">The property name to modify</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool RemoveProperty<T>(string name)
        {
            return RemoveProperty<T>(template | name.Fnv32());
        }
        /// <summary>
        /// Removes an existing property from the given flex object
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool RemoveProperty<T>()
        {
            return RemoveProperty<T>(template | typeof(T).FullName.Fnv32());
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new FlexBinder(this, true, parameter);
        }
    }
}
