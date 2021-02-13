// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime.Serialization
{
    public static partial class TypeFormatter
    {
        private readonly static Type DeserializationTarget = typeof(IDeserialializationTarget);
        private readonly static Type SetterType;
        private readonly static MethodInfo SetterInvoke;

        private static void DefineDeserializer(Type type, TypeBuilder builder, IEnumerable<FieldInfo> fields, ref Dictionary<FieldBuilder, FieldInfo> fieldAccessors)
        {
            bool referenceType = type.IsClass;
            bool addCallbacks = DeserializationTarget.IsAssignableFrom(type);
            MethodInfo deserializeMethod = typeof(TypeFormatter).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new Type[]
            {
                typeof(Stream),
                typeof(int),
                typeof(Type)

            }, null);
            MethodInfo getType = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            MethodBuilder method = builder.DefineMethod("Deserialize", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), new Type[]
            {
                typeof(Stream)

            });
            ILGenerator gen = method.GetILGenerator();
            {
                gen.DeclareLocal(type);
                gen.DeclareLocal(typeof(object));
                if (referenceType)
                {
                    gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                    gen.Emit(OpCodes.Stloc, 0);
                    if (addCallbacks)
                    {
                        gen.Emit(OpCodes.Ldloc, 0);
                        gen.EmitCall(OpCodes.Callvirt, DeserializationTarget.GetMethod("OnBeforeDeserialization"), null);
                    }
                }
                else
                {
                    gen.Emit(OpCodes.Ldloca_S, 0);
                    gen.Emit(OpCodes.Initobj, type);
                    if (addCallbacks)
                    {
                        gen.Emit(OpCodes.Ldloca_S, 0);
                        gen.Emit(OpCodes.Box, type);
                        gen.EmitCall(OpCodes.Callvirt, DeserializationTarget.GetMethod("OnBeforeDeserialization"), null);
                    }
                }
                foreach (FieldInfo field in fields)
                {
                    if (!field.IsPublic)
                    {
                        if (!referenceType)
                        {
                            continue;
                        }
                        if (fieldAccessors == null)
                        {
                            fieldAccessors = CollectionPool<Dictionary<FieldBuilder, FieldInfo>, FieldBuilder, FieldInfo>.Get();
                        }
                        FieldBuilder setter = builder.DefineField(string.Concat("_set", field.Name), SetterType, FieldAttributes.Private);
                        fieldAccessors.Add(setter, field);

                        gen.Emit(OpCodes.Ldarg_0);
                        gen.Emit(OpCodes.Ldfld, setter);
                        gen.Emit(OpCodes.Ldloc, 0);
                        gen.Emit(OpCodes.Ldarg_1);
                        gen.Emit(OpCodes.Ldc_I4_S, field.GetAttribute<SerializedAttribute>().Index);
                        gen.Emit(OpCodes.Ldtoken, field.FieldType);
                        gen.EmitCall(OpCodes.Call, getType, null);
                        gen.EmitCall(OpCodes.Call, deserializeMethod, null);
                        gen.EmitCall(OpCodes.Callvirt, SetterInvoke, null);
                    }
                    else
                    {
                        if (!referenceType)
                        {
                            gen.Emit(OpCodes.Ldloca_S, 0);
                        }
                        else gen.Emit(OpCodes.Ldloc, 0);
                        gen.Emit(OpCodes.Ldarg_1);
                        gen.Emit(OpCodes.Ldc_I4_S, field.GetAttribute<SerializedAttribute>().Index);
                        gen.Emit(OpCodes.Ldtoken, field.FieldType);
                        gen.EmitCall(OpCodes.Call, getType, null);
                        gen.EmitCall(OpCodes.Call, deserializeMethod, null);
                        if (!field.FieldType.IsClass)
                        {
                            gen.Emit(OpCodes.Unbox_Any, field.FieldType);
                        }
                        gen.Emit(OpCodes.Stfld, field);
                    }
                }
                gen.Emit(OpCodes.Ldloc, 0);
                gen.Emit(OpCodes.Box, type);
                gen.Emit(OpCodes.Stloc, 1);

                if (addCallbacks)
                {
                    gen.Emit(OpCodes.Ldloc, 1);
                    gen.EmitCall(OpCodes.Callvirt, DeserializationTarget.GetMethod("OnAfterDeserialization"), null);
                }

                gen.Emit(OpCodes.Ldloc, 1);
                gen.Emit(OpCodes.Ret);
            }
            builder.DefineMethodOverride(method, InterfaceType.GetMethod("Deserialize"));
        }

        /// <summary>
        /// Deserializes the data on the provided stream for a given marker and reconstitutes the graph of objects
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <param name="requestedMarkerId">A marker targeted to deserialize</param>
        /// <param name="fieldType">The type of the field to deserialize</param>
        /// <returns>The top object of the deserialized graph or a default value</returns>
        public static object Deserialize(Stream serializationStream, int requestedMarkerId, Type fieldType)
        {
            long position = serializationStream.Position;

        Head:
            UInt32 typeId = serializationStream.ToVariableInt();
            switch ((TypeCodes)typeId)
            {
                case TypeCodes.Marker:
                    {
                        int id = (int)serializationStream.ToVariableInt();
                        if (id < requestedMarkerId)
                        {
                            int offset = (int)serializationStream.ToVariableInt();
                            serializationStream.Position += offset;
                        }
                        else if (id > requestedMarkerId)
                        {
                            serializationStream.Position = position;
                            return fieldType.GetDefault();
                        }
                        else serializationStream.ToVariableInt();
                        goto Head;
                    }
                default: return Deserialize(serializationStream, typeId, fieldType);
            }
        }
        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <param name="typeId">A specific type ID to deserialize data for</param>
        /// <param name="fieldType">An optional type of the field to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static object Deserialize(Stream serializationStream, UInt32 typeId, Type fieldType = null)
        {

        Head:
            switch ((TypeCodes)typeId)
            {
                case TypeCodes.Empty: return null;
                case TypeCodes.TrueConstant: return true;
                case TypeCodes.FalseConstant: return false;
                case TypeCodes.Boolean:
                    {
                        typeId = serializationStream.Get();
                        goto Head;
                    }
                case TypeCodes.Byte: return PrimitiveFormatter.ReadByte(serializationStream);
                case TypeCodes.SByte: return PrimitiveFormatter.ReadSByte(serializationStream);
                case TypeCodes.Char: return PrimitiveFormatter.ReadChar(serializationStream);
                case TypeCodes.Int16: return PrimitiveFormatter.ReadInt16(serializationStream);
                case TypeCodes.UInt16: return PrimitiveFormatter.ReadUInt16(serializationStream);
                case TypeCodes.Int32: return PrimitiveFormatter.ReadInt32(serializationStream);
                case TypeCodes.UInt32: return PrimitiveFormatter.ReadUInt32(serializationStream);
                case TypeCodes.Int64: return PrimitiveFormatter.ReadInt64(serializationStream);
                case TypeCodes.UInt64: return PrimitiveFormatter.ReadUInt64(serializationStream);
                case TypeCodes.Single: return PrimitiveFormatter.ReadSingle(serializationStream);
                case TypeCodes.Double: return PrimitiveFormatter.ReadDouble(serializationStream);
                case TypeCodes.Decimal: return PrimitiveFormatter.ReadDecimal(serializationStream);
                case TypeCodes.DateTime: return PrimitiveFormatter.ReadDateTime(serializationStream);
                case TypeCodes.String: return StringFormatter.Read(serializationStream);
                case TypeCodes.Guid: return PrimitiveFormatter.ReadGuid(serializationStream);
                case TypeCodes.Array: return ArrayFormatter.Read(serializationStream, fieldType);
                case TypeCodes.Collection: return CollectionFormatter.Read(serializationStream, fieldType);
                case TypeCodes.Dictionary: return DictionaryFormatter.Read(serializationStream, fieldType);
                case TypeCodes.Exception: return ExceptionFormatter.Read(serializationStream);
                default:
                    {
                        ITypeFormatter formatter;
                        cacheLock.ReadLock();
                        try
                        {
                            if (!typeCache.TryGetValue(typeId, out formatter))
                                throw new TypeLoadException();
                        }
                        finally
                        {
                            cacheLock.ReadRelease();
                        }
                        return formatter.Deserialize(serializationStream);
                    }
            }
        }
    }
}
