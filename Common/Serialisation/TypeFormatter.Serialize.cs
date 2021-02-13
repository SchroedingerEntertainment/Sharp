// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime.Serialization
{
    public static partial class TypeFormatter
    {
        private readonly static Type SerializationTarget = typeof(ISerialializationTarget);
        private readonly static Type GetterType;
        private readonly static MethodInfo GetterInvoke;

        private static void DefineSerializer(Type type, TypeBuilder builder, IEnumerable<FieldInfo> fields, ref Dictionary<FieldBuilder, FieldInfo> fieldAccessors)
        {
            bool referenceType = type.IsClass;
            bool useMemoryPool = enableMemoryPooling;
            bool addCallbacks = SerializationTarget.IsAssignableFrom(type);
            Type returnType = typeof(void);
            Type[] parameterTypes = new Type[]
            {
                typeof(Stream),
                typeof(object)

            };
            MethodInfo serializeMethod = typeof(TypeFormatter).GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, parameterTypes, null);
            MethodInfo serializeMarker = typeof(TypeFormatter).GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new Type[]
            {
                typeof(Stream),
                typeof(Stream),
                typeof(int)

            }, null);
            Type memoryStream = typeof(MemoryStream);
            Type memoryPool = typeof(MemoryPool<MemoryStream>);
            MethodInfo memoryStreamClear = typeof(StreamExtension).GetMethod("Clear", BindingFlags.Public | BindingFlags.Static);
            MethodBuilder method = builder.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes);
            ILGenerator gen = method.GetILGenerator();
            {
                gen.DeclareLocal(memoryStream);
                if (useMemoryPool)
                {
                    gen.EmitCall(OpCodes.Call, memoryPool.GetMethod("Get"), null);
                }
                else gen.Emit(OpCodes.Newobj, memoryStream.GetConstructor(Type.EmptyTypes));
                gen.Emit(OpCodes.Stloc, 0);
                gen.BeginExceptionBlock();
                if (addCallbacks)
                {
                    gen.Emit(OpCodes.Ldarg_2);
                    gen.EmitCall(OpCodes.Callvirt, SerializationTarget.GetMethod("OnBeforeSerialization"), null);
                }
                foreach (FieldInfo field in fields)
                {
                    if (!referenceType && !field.IsPublic)
                    {
                        continue;
                    }
                    gen.Emit(OpCodes.Ldloc, 0);
                    gen.EmitCall(OpCodes.Call, memoryStreamClear, null);
                    gen.Emit(OpCodes.Ldloc, 0);
                    if (!field.IsPublic)
                    {
                        if (fieldAccessors == null)
                        {
                            fieldAccessors = CollectionPool<Dictionary<FieldBuilder, FieldInfo>, FieldBuilder, FieldInfo>.Get();
                        }
                        FieldBuilder getter = builder.DefineField(string.Concat("_get", field.Name), GetterType, FieldAttributes.Private);
                        fieldAccessors.Add(getter, field);

                        gen.Emit(OpCodes.Ldarg_0);
                        gen.Emit(OpCodes.Ldfld, getter);
                        gen.Emit(OpCodes.Ldarg_2);
                        gen.EmitCall(OpCodes.Callvirt, GetterInvoke, null);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Ldarg_2);
                        if (!type.IsClass)
                        {
                            gen.Emit(OpCodes.Unbox_Any, type);
                        }
                        gen.Emit(OpCodes.Ldfld, field);
                        if (!field.FieldType.IsClass)
                        {
                            gen.Emit(OpCodes.Box, field.FieldType);
                        }
                    }
                    gen.EmitCall(OpCodes.Call, serializeMethod, null);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Ldloc, 0);
                    gen.Emit(OpCodes.Ldc_I4_S, field.GetAttribute<SerializedAttribute>().Index);
                    gen.EmitCall(OpCodes.Call, serializeMarker, null);
                }
                if (addCallbacks)
                {
                    gen.Emit(OpCodes.Ldarg_2);
                    gen.EmitCall(OpCodes.Callvirt, SerializationTarget.GetMethod("OnAfterSerialization"), null);
                }
                gen.BeginFinallyBlock();
                gen.Emit(OpCodes.Ldloc, 0);
                if (useMemoryPool)
                {
                    gen.EmitCall(OpCodes.Call, memoryPool.GetMethod("Return"), null);
                }
                else gen.EmitCall(OpCodes.Callvirt, memoryStream.GetMethod("Dispose"), null);
                gen.EndExceptionBlock();
                gen.Emit(OpCodes.Ret);
            }
            builder.DefineMethodOverride(method, InterfaceType.GetMethod("Serialize"));
        }

        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a 
        /// variety of backing stores (such as files, network, memory, and so on)
        /// </param>
        /// <param name="typeId">A defined type ID to find a serializer for this object if necessary</param>
        /// <param name="graph">
        /// The object, or root of the object graph, to serialize. All child objects of this 
        /// root object are automatically serialized
        /// </param>
        public static void Serialize(Stream serializationStream, UInt32 typeId, object graph, bool addTypeCode = true)
        {
            if (graph != null)
            {
                switch (GetTypeCodes(graph.GetType()))
                {
                    case TypeCodes.Boolean: 
                        PrimitiveFormatter.WriteBoolean(serializationStream, (bool)graph);
                        break;
                    case TypeCodes.Byte: 
                        PrimitiveFormatter.WriteByte(serializationStream, (byte)graph, addTypeCode);
                        break;
                    case TypeCodes.SByte:
                        PrimitiveFormatter.WriteSByte(serializationStream, (SByte)graph, addTypeCode);
                        break;
                    case TypeCodes.Char:
                        PrimitiveFormatter.WriteChar(serializationStream, (char)graph, addTypeCode);
                        break;
                    case TypeCodes.Int16:
                        PrimitiveFormatter.WriteInt16(serializationStream, (Int16)graph, addTypeCode);
                        break;
                    case TypeCodes.UInt16:
                        PrimitiveFormatter.WriteUInt16(serializationStream, (UInt16)graph, addTypeCode);
                        break;
                    case TypeCodes.Int32:
                        PrimitiveFormatter.WriteInt32(serializationStream, (Int32)graph, addTypeCode);
                        break;
                    case TypeCodes.UInt32:
                        PrimitiveFormatter.WriteUInt32(serializationStream, (UInt32)graph, addTypeCode);
                        break;
                    case TypeCodes.Int64:
                        PrimitiveFormatter.WriteInt64(serializationStream, (Int64)graph, addTypeCode);
                        break;
                    case TypeCodes.UInt64:
                        PrimitiveFormatter.WriteUInt64(serializationStream, (UInt64)graph, addTypeCode);
                        break;
                    case TypeCodes.Single:
                        PrimitiveFormatter.WriteSingle(serializationStream, (float)graph, addTypeCode);
                        break;
                    case TypeCodes.Double:
                        PrimitiveFormatter.WriteDouble(serializationStream, (double)graph, addTypeCode);
                        break;
                    case TypeCodes.Decimal:
                        PrimitiveFormatter.WriteDecimal(serializationStream, (decimal)graph, addTypeCode);
                        break;
                    case TypeCodes.DateTime:
                        PrimitiveFormatter.WriteDateTime(serializationStream, (DateTime)graph, addTypeCode);
                        break;
                    case TypeCodes.String:
                        StringFormatter.Write(serializationStream, graph as string, addTypeCode);
                        break;
                    case TypeCodes.Guid:
                        PrimitiveFormatter.WriteGuid(serializationStream, (Guid)graph, addTypeCode);
                        break;
                    case TypeCodes.Array: 
                        ArrayFormatter.Write(serializationStream, graph as Array, addTypeCode);
                        break;
                    case TypeCodes.Collection: 
                        CollectionFormatter.Write(serializationStream, graph as ICollection, addTypeCode);
                        break;
                    case TypeCodes.Dictionary: 
                        DictionaryFormatter.Write(serializationStream, graph as IDictionary, addTypeCode);
                        break;
                    case TypeCodes.Exception:
                        ExceptionFormatter.Write(serializationStream, graph as Exception, addTypeCode);
                        break;
                    default:
                        {
                            ITypeFormatter formatter;
                            cacheLock.ReadLock();
                            try
                            {
                                if (!typeCache.TryGetValue(typeId, out formatter))
                                    throw new SerializationException();
                            }
                            finally
                            {
                                cacheLock.ReadRelease();
                            }
                            if (addTypeCode)
                            {
                                serializationStream.EncodeVariableInt(typeId);
                            }
                            formatter.Serialize(serializationStream, graph);
                        }
                        break;
                }
            }
            else serializationStream.Put((byte)TypeCode.Empty);
        }
    }
}
