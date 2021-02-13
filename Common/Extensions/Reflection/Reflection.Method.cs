// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Reflection
{
    public static partial class ReflectionExtension
    {
        /// <summary>
        /// Returns a list from this assembly containing any method with a given attribute attached to
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>The list of methofds found in this assembly</returns>
        public static MethodInfo[] GetMethods<T>(this Assembly assembly) where T : Attribute
        {
            Type attribType = typeof(T);

            List<MethodInfo> result = CollectionPool<List<MethodInfo>, MethodInfo>.Get();
            try
            {
                foreach (Type type in assembly.GetTypes())
                    foreach (MethodInfo minf in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        if (minf.GetCustomAttributes(attribType, true).Length > 0)
                            result.Add(minf);

                return result.ToArray();
            }
            finally
            {
                CollectionPool<List<MethodInfo>, MethodInfo>.Return(result);
            }
        }
        /// <summary>
        /// Returns a list from this assembly containing any method with a given attribute attached to
        /// </summary>
        /// <typeparam name="T">Type of the attribute to search for</typeparam>
        /// <returns>The list of methods found in this assembly</returns>
        public static MethodInfo[] GetMethods<T>(this Type type, BindingFlags flags) where T : Attribute
        {
            Type attribType = typeof(T);

            List<MethodInfo> result = CollectionPool<List<MethodInfo>, MethodInfo>.Get();
            try
            {
                foreach (MethodInfo minf in type.GetMethods(flags))
                    if (minf.GetCustomAttributes(attribType, true).Length > 0)
                        result.Add(minf);

                return result.ToArray();
            }
            finally
            {
                CollectionPool<List<MethodInfo>, MethodInfo>.Return(result);
            }
        }
    }
}
