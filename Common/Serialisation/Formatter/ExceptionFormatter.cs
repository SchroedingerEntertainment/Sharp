// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides base functionality for the Exception serialization formatter
    /// </summary>
    public static class ExceptionFormatter
    {
        public const string TypeName = "TypeName";
        public const string Message = "Message";
        public const string Source = "Source";
        public const string StackTrace = "StackTrace";

        private readonly static Type DictionaryType = typeof(Dictionary<string, object>);

        /// <summary>
        /// Serializes a data object to the provided stream
        /// </summary>
        /// <param name="serializationStream">
        /// The stream where the formatter puts the serialized data. This stream can reference a 
        /// variety of backing stores (such as files, network, memory, and so on)
        /// </param>
        /// <param name="value">
        /// The object, or root of the object graph, to serialize. All child objects of this 
        /// root object are automatically serialized
        /// </param>
        /// <param name="addTypeCode">Defines if a type code flag should be set in front of the serialized data</param>
        public static void Write(Stream serializationStream, Exception value, bool addTypeCode)
        {
            if (addTypeCode)
            {
                serializationStream.Put((byte)TypeCodes.Exception);
            }
            IDictionary formattable = CollectionPool<Dictionary<string, object>, string, object>.Get();
            try
            {
                formattable.Add(TypeName, value.GetType().FullName);
                formattable.Add(Message, value.Message);
                formattable.Add(Source, value.Source);
                formattable.Add(StackTrace, value.StackTrace);
                foreach (DictionaryEntry item in value.Data)
                {
                    formattable.Add(item.Key, item.Value);
                }
                TypeFormatter.Serialize(serializationStream, (UInt32)TypeCodes.Dictionary, formattable, false);
            }
            finally
            {
                CollectionPool<Dictionary<string, object>, string, object>.Return(formattable as Dictionary<string, object>);
            }
        }
        /// <summary>
        /// Deserializes a data object from the provided stream
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize</param>
        /// <returns>The top object of the deserialized graph</returns>
        public static Exception Read(Stream serializationStream)
        {
            IDictionary formattable = TypeFormatter.Deserialize(serializationStream, (UInt32)TypeCodes.Dictionary, DictionaryType) as IDictionary;
            try
            {
                SerializedExceptionContext value = new SerializedExceptionContext(formattable[Message] as string, formattable[StackTrace] as string);
                formattable.Remove(Message);
                formattable.Remove(StackTrace);

                value.ExceptionType = Type.GetType(formattable[TypeName] as string, true, true);
                formattable.Remove(TypeName);
                
                value.Source = formattable[Source] as string;
                formattable.Remove(Source);

                foreach (DictionaryEntry item in formattable)
                {
                    value.Data.Add(item.Key, item.Value);
                }
                return value;
            }
            finally
            {
                CollectionPool<Dictionary<string, object>, string, object>.Return(formattable as Dictionary<string, object>);
            }
        }
    }
}
