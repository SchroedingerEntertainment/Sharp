// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides binary serialization of built-in and custom types
    /// </summary>
    public static partial class TypeFormatter
    {
        class DataIndexComparer : IComparer<FieldInfo>
        {
            public int Compare(FieldInfo x, FieldInfo y)
            {
                return x.GetAttribute<SerializedAttribute>().Index.CompareTo(
                       y.GetAttribute<SerializedAttribute>().Index);
            }
        }

        private readonly static DataIndexComparer Comparer = new DataIndexComparer();
        private readonly static Dictionary<UInt32, ITypeFormatter> typeCache;
        private static ReadWriteLock cacheLock;

        private static bool enableMemoryPooling;
        /// <summary>
        /// Gets or sets if auto formatting uses the memory pool (best for frequent
        /// serialization operations)
        /// </summary>
        public static bool EnableMemoryPooling
        {
            get { return enableMemoryPooling; }
            set { enableMemoryPooling = value; }
        }

        static TypeFormatter()
        {
            enableMemoryPooling = true;

            GetterType = typeof(Func<object, object>);
            GetterInvoke = GetterType.GetMethod("Invoke");

            SetterType = typeof(Action<object, object>);
            SetterInvoke = SetterType.GetMethod("Invoke");

            typeCache = new Dictionary<UInt32, ITypeFormatter>();
            cacheLock = new ReadWriteLock();

            Register(GetTypeId(typeof(PathDescriptor)), new PathFormatter());
            Register(GetTypeId(typeof(FileDescriptor)), new FileFormatter());
        }

        /// <summary>
        /// Assigns a new formatter instance to a given type ID if possible
        /// </summary>
        /// <param name="typeId">A user defined type ID to assign the formatter to</param>
        /// <param name="formatter">A type formatter handling serialization operations</param>
        /// <returns>True if successfully assigned, false otherwise</returns>
        public static bool Register(UInt32 typeId, ITypeFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            cacheLock.ReadLock();
            try
            {
                if (typeCache.ContainsKey(typeId))
                    return false;
            }
            finally
            {
                cacheLock.ReadRelease();
            }

            cacheLock.WriteLock();
            try
            {
                if (!typeCache.ContainsKey(typeId))
                {
                    typeCache.Add(typeId, formatter);
                }
                else return false;
            }
            finally
            {
                cacheLock.WriteRelease();
            }
            return true;
        }
        /// <summary>
        /// Assigns a new formatter to a given type ID if possible
        /// </summary>
        /// <param name="typeId">A user defined type ID to assign the formatter to</param>
        /// <param name="type">A type to generate a formatter for</param>
        /// <returns>True if successfully assigned, false otherwise</returns>
        public static bool Register(UInt32 typeId, Type type)
        {
            cacheLock.ReadLock();
            try
            {
                if (typeCache.ContainsKey(typeId))
                    return false;
            }
            finally
            {
                cacheLock.ReadRelease();
            }

            cacheLock.WriteLock();
            try
            {
                if (!typeCache.ContainsKey(typeId))
                {
                    ITypeFormatter formatter = CreateFormatter(type);
                    typeCache.Add(typeId, formatter);
                }
                else return false;
            }
            finally
            {
                cacheLock.WriteRelease();
            }
            return true;
        }
        /// <summary>
        /// Assigns a new formatter to a given type ID if possible
        /// </summary>
        /// <param name="type">A type to generate a formatter for</param>
        /// <returns>True if successfully assigned, false otherwise</returns>
        public static bool Register(Type type)
        {
            return Register(GetTypeId(type), type);
        }
        /// <summary>
        /// Assigns a new formatter to a given type ID if possible
        /// </summary>
        public static bool Register<T>()
        {
            return Register(typeof(T));
        }

        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a 
        /// variety of backing stores (such as files, network, memory, and so on)
        /// </param>
        /// <param name="graph">
        /// The object, or root of the object graph, to serialize. All child objects of this 
        /// root object are automatically serialized
        /// </param>
        public static void Serialize(Stream serializationStream, object graph)
        {
            if (graph != null)
            {
                Serialize(serializationStream, GetTypeId(graph.GetType()), graph);
            }
            else serializationStream.Put((byte)TypeCode.Empty);
        }

        /// <summary>
        /// Serializes a section marker of arbitary length to the provided stream
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a 
        /// variety of backing stores (such as files, network, memory, and so on)
        /// </param>
        /// <param name="markerData">The stream where data is defined that should be enclosed by the marker</param>
        /// <param name="markerId">An ID the marker is identifed by in the formatted stream</param>
        public static void Serialize(Stream serializationStream, Stream markerData, int markerId)
        {
            serializationStream.Put((byte)TypeCodes.Marker);
            serializationStream.EncodeVariableInt((UInt32)markerId);
            serializationStream.EncodeVariableInt((UInt32)markerData.Length);
            
            markerData.Position = 0;
            markerData.CopyTo(serializationStream);
        }

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static object Deserialize(Stream serializationStream)
        {
            return Deserialize(serializationStream, -1, typeof(object));
        }

        /// <summary>
        /// Releases an assigned formatter bound to the given type ID
        /// </summary>
        /// <param name="typeId">A user defined type ID to remove the formatter</param>
        /// <returns>True if successfully removed, false otherwise</returns>
        public static bool Release(UInt32 typeId)
        {
            cacheLock.WriteLock();
            try
            {
                return typeCache.Remove(typeId);
            }
            finally
            {
                cacheLock.WriteRelease();
            }
        }
        /// <summary>
        /// Releases an assigned formatter bound to the given type
        /// </summary>
        /// <param name="type">A type to remove the formatter</param>
        /// <returns>True if successfully removed, false otherwise</returns>
        public static bool Release(Type type)
        {
            return Release(GetTypeId(type));
        }
    }
}
