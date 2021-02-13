// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Reflection
{
    public static partial class ReflectionExtension
    {
        /// <summary>
        /// Returns a property from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info instance or null</returns>
        public static PropertyInfo GetProperty<T>(this Type type)
        {
            Type attribType = typeof(T);

            foreach (PropertyInfo property in type.GetProperties())
                if (property.GetCustomAttributes(attribType, true).Length > 0)
                    return property;

            return null;
        }
        /// <summary>
        /// Returns a property from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info instance or null</returns>
        public static PropertyInfo GetProperty<T>(this Type type, BindingFlags flags)
        {
            Type attribType = typeof(T);

            foreach (PropertyInfo property in type.GetProperties(flags))
                if (property.GetCustomAttributes(attribType, true).Length > 0)
                    return property;

            return null;
        }
        /// <summary>
        /// Returns a property list from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info list instance or null</returns>
        public static PropertyInfo[] GetProperties<T>(this Type type)
        {
            List<PropertyInfo> result = CollectionPool<List<PropertyInfo>, PropertyInfo>.Get();
            try
            {
                Type attribType = typeof(T);

                foreach (PropertyInfo property in type.GetProperties())
                    if (property.GetCustomAttributes(attribType, true).Length > 0)
                        result.Add(property);

                return result.ToArray();
            }
            finally
            {
                CollectionPool<List<PropertyInfo>, PropertyInfo>.Return(result);
            }
        }
        /// <summary>
        /// Returns a property list from this type with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A property info list instance or null</returns>
        public static PropertyInfo[] GetProperties<T>(this Type type, BindingFlags flags)
        {
            List<PropertyInfo> result = CollectionPool<List<PropertyInfo>, PropertyInfo>.Get();
            try
            {
                Type attribType = typeof(T);

                foreach (PropertyInfo property in type.GetProperties(flags))
                    if (property.GetCustomAttributes(attribType, true).Length > 0)
                        result.Add(property);

                return result.ToArray();
            }
            finally
            {
                CollectionPool<List<PropertyInfo>, PropertyInfo>.Return(result);
            }
        }
    }
}
