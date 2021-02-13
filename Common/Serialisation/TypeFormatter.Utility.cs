// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime.Serialization
{
    public static partial class TypeFormatter
    {
        private readonly static Type InterfaceType = typeof(ITypeFormatter);
        private readonly static Type CollectionType = typeof(ICollection);
        private readonly static Type DictionaryType = typeof(IDictionary);
        private readonly static Type ExceptionType = typeof(Exception);
        private readonly static Type GuidType = typeof(Guid);

        private static ITypeFormatter CreateFormatter(Type type)
        {
            Dictionary<FieldBuilder, FieldInfo> setter = null;
            Dictionary<FieldBuilder, FieldInfo> getter = null;
            try
            {
                IEnumerable<FieldInfo> fields = type.GetFields<SerializedAttribute>(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Sort(Comparer);
                if (fields.Any())
                {
                    #if NET_FRAMEWORK
                    AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("TypeFormatter"), AssemblyBuilderAccess.Run);
                    #else
                    AssemblyBuilder asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("TypeFormatter"), AssemblyBuilderAccess.Run);
                    #endif

                    TypeBuilder builder = asm.DefineDynamicModule("Serialization").DefineType(string.Format("{0}.{1}", type.Name, Guid.NewGuid()));
                    builder.AddInterfaceImplementation(InterfaceType);

                    DefineDeserializer(type, builder, fields, ref setter);
                    DefineSerializer(type, builder, fields, ref getter);

                    #if NET_FRAMEWORK
                    ITypeFormatter instance = Activator.CreateInstance(builder.CreateType()) as ITypeFormatter;
                    #else
                    ITypeFormatter instance = Activator.CreateInstance(builder.CreateTypeInfo()) as ITypeFormatter;
                    #endif

                    if (setter != null)
                    {
                        type = instance.GetType();
                        foreach (KeyValuePair<FieldBuilder, FieldInfo> accessor in setter)
                            type.GetField(accessor.Key.Name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, accessor.Value.CreateSetter());
                    }
                    if (getter != null)
                    {
                        type = instance.GetType();
                        foreach (KeyValuePair<FieldBuilder, FieldInfo> accessor in getter)
                            type.GetField(accessor.Key.Name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, accessor.Value.CreateGetter());
                    }
                    return instance;
                }
                else throw new TypeInitializationException(type.FullName, new ArgumentOutOfRangeException("SerializedAttribute"));
            }
            finally
            {
                if (setter != null)
                {
                    CollectionPool<Dictionary<FieldBuilder, FieldInfo>, FieldBuilder, FieldInfo>.Return(setter);
                }
                if (getter != null)
                {
                    CollectionPool<Dictionary<FieldBuilder, FieldInfo>, FieldBuilder, FieldInfo>.Return(getter);
                }
            }
        }

        /// <summary>
        /// Determines the formatter code of the provided type
        /// </summary>
        public static TypeCodes GetTypeCodes(Type type)
        {
            TypeCode code = Type.GetTypeCode(type);
            switch ((TypeCodes)code)
            {
                case TypeCodes.Empty:
                case TypeCodes.Boolean:
                case TypeCodes.Byte:
                case TypeCodes.SByte:
                case TypeCodes.Char:
                case TypeCodes.Int16:
                case TypeCodes.UInt16:
                case TypeCodes.Int32:
                case TypeCodes.UInt32:
                case TypeCodes.Int64:
                case TypeCodes.UInt64:
                case TypeCodes.Single:
                case TypeCodes.Double:
                case TypeCodes.Decimal:
                case TypeCodes.DateTime:
                case TypeCodes.String: return (TypeCodes)code;
                default: switch(code)
                    {
                        case TypeCode.Object:
                            {
                                if (type.IsArray)
                                {
                                    return TypeCodes.Array;
                                }
                                else if (DictionaryType.IsAssignableFrom(type))
                                {
                                    return TypeCodes.Dictionary;
                                }
                                else if (CollectionType.IsAssignableFrom(type))
                                {
                                    return TypeCodes.Collection;
                                }
                                if (ExceptionType.IsAssignableFrom(type))
                                {
                                    return TypeCodes.Exception;
                                }
                                if (GuidType.IsAssignableFrom(type))
                                {
                                    return TypeCodes.Guid;
                                }
                                else return TypeCodes.Object;
                            }
                        default: return TypeCodes.Empty;
                    }
            }
        }
        private static UInt32 GetTypeId(Type type)
        {
            return type.FullName.Fnv32();
        }

        /// <summary>
        /// Determines if the provided type can be serialized by the formatter
        /// </summary>
        public static bool CanSerialize(Type type)
        {
            switch (GetTypeCodes(type))
            {
                case TypeCodes.Boolean:
                case TypeCodes.Byte:
                case TypeCodes.SByte:
                case TypeCodes.Char:
                case TypeCodes.Int16:
                case TypeCodes.UInt16:
                case TypeCodes.Int32:
                case TypeCodes.UInt32:
                case TypeCodes.Int64:
                case TypeCodes.UInt64:
                case TypeCodes.Single:
                case TypeCodes.Double:
                case TypeCodes.Decimal:
                case TypeCodes.DateTime:
                case TypeCodes.String:
                case TypeCodes.Guid:
                case TypeCodes.Array:
                case TypeCodes.Collection:
                case TypeCodes.Dictionary:
                case TypeCodes.Exception: return true;
                default:
                    {
                        cacheLock.ReadLock();
                        try
                        {
                            return typeCache.ContainsKey(GetTypeId(type));
                        }
                        finally
                        {
                            cacheLock.ReadRelease();
                        }
                    }
            }
        }
    }
}
