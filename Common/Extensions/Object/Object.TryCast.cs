// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System
{
    public static partial class ObjectExtension
    {
        /// <summary>
        /// Tries to convert this type into another
        /// </summary>
        /// <param name="result">The converted type-value instance</param>
        /// <returns>True if conversion was successfull, false otherwise</returns>
        public static bool TryCast<T>(this object instance, out T result)
        {
            if (instance is T)
            {
                result = (T)instance;
                return true;
            }
            else
            {
                Type type = typeof(T);
                if (instance != null)
                {
                    var converter = TypeDescriptor.GetConverter(type);
                    if (!converter.CanConvertFrom(instance.GetType()))
                    {
                        try
                        {
                            result = (T)converter.ConvertFrom(instance);
                            return true;
                        }
                        catch
                        { }
                    }
                    try
                    {
                        result = (T)Convert.ChangeType(instance, type);
                        return true;
                    }
                    catch
                    {
                        result = default(T);
                        return false;
                    }
                }
                result = default(T);
                return !type.IsValueType;
            }
        }
        /// <summary>
        /// Tries to convert this type into another
        /// </summary>
        /// <param name="result">The converted type-value instance</param>
        /// <returns>True if conversion was successfull, false otherwise</returns>
        public static bool TryCast(this object instance, Type type, out object result)
        {
            if (type.IsAssignableFrom(instance.GetType()))
            {
                result = instance;
                return true;
            }
            else
            {
                if (instance != null)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if (converter.CanConvertFrom(instance.GetType()))
                    {
                        try
                        {
                            result = converter.ConvertFrom(instance);
                            return true;
                        }
                        catch
                        { }
                    }
                    try
                    {
                        result = Convert.ChangeType(instance, type);
                        return true;
                    }
                    catch
                    {
                        result = null;
                        return false;
                    }
                }
                result = null;
                return false;
            }
        }
    }
}