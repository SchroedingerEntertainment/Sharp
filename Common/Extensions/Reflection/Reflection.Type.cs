// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Reflection
{
    public static partial class ReflectionExtension
    {
        /// <summary>
        /// Returns a type from this assembly with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A type instance or null</returns>
        public static Type GetType<T>(this Assembly assembly, bool inherit = false)
        {
            Type reference = typeof(T);
            if (reference.IsAttribute())
            {
                foreach (Type type in assembly.GetTypes())
                    if (type.IsDefined(reference, inherit))
                        return type;
            }
            else
            {
                foreach (Type type in assembly.GetTypes())
                    if (reference.IsAssignableFrom(type))
                        return type;
            }
            return null;
        }
        /// <summary>
        /// Returns a type list from this assembly with a given attribute attached to if existing
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>A type list instance or null</returns>
        public static Type[] GetTypes<T>(this Assembly assembly, bool inherit = false)
        {
            List<Type> result = CollectionPool<List<Type>, Type>.Get();
            try
            {
                Type reference = typeof(T);
                if (reference.IsAttribute())
                {
                    foreach (Type type in assembly.GetTypes())
                        if (type.IsDefined(reference, inherit))
                            result.Add(type);
                }
                else
                {
                    foreach (Type type in assembly.GetTypes())
                        if (reference.IsAssignableFrom(type))
                            result.Add(type);
                }
                return result.ToArray();
            }
            finally
            {
                CollectionPool<List<Type>, Type>.Return(result);
            }
        }

        /// <summary>
        /// Walks the inheritance chain of this type and tries to find a desired base
        /// </summary>
        /// <param name="desiredBase">A base type this type is inheriting from</param>
        /// <returns>The type that first inherited the desired base if available, this type otherwise</returns>
        public static Type GetType(this Type type, Type desiredBase)
        {
            while (desiredBase.IsAssignableFrom(type.BaseType))
                type = type.BaseType;

            return type;
        }
        /// <summary>
        /// Walks the inheritance chain of this type and tries to find a desired base
        /// </summary>
        /// <param name="desiredBase">A base type this type is inheriting from</param>
        /// <returns>The type that first inherited the desired base if available, this type otherwise</returns>
        public static Type GetType<T>(this Type type)
        {
            return GetType(type, typeof(T));
        }

        /// <summary>
        /// Returns the underlaying type of an abstract member
        /// </summary>
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event: return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field: return ((FieldInfo)member).FieldType;
                case MemberTypes.Method: return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property: return ((PropertyInfo)member).PropertyType;
                default: return null;
            }
        }
    }
}
