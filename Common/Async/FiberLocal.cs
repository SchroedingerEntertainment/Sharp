// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

#if NET_FRAMEWORK
using System.Runtime.Remoting.Messaging;
#else
using System.Collections.Concurrent;
#endif

namespace System.Threading
{
    /// <summary>
    /// A typed Thread and Task chain unique value container
    /// </summary>
    public sealed class FiberLocal<T>
    {
        #if !NET_FRAMEWORK
        private static ConcurrentDictionary<Guid, AsyncLocal<object>> states;
        #endif

        Guid id;

        /// <summary>
        /// Gets or sets a value of type T in Thread and Task local scope
        /// </summary>
        public T Value
        {
            get { return (T)GetData(); }
            set { SetData(value); }
        }

        /// <summary>
        /// Gets if there is a value in this local scope
        /// </summary>
        public bool HasValue
        {
            get
            {
                object tmp = GetData();
                return (tmp != null && tmp is T);
            }
        }

        static FiberLocal()
        {
            #if !NET_FRAMEWORK
            states = new ConcurrentDictionary<Guid, AsyncLocal<object>>();
            #endif
        }
        /// <summary>
        /// Initializes a new thread and task aware variable
        /// </summary>
        public FiberLocal()
        {
            this.id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the raw data that is stored in this local scope
        /// </summary>
        /// <returns>An object or null</returns>
        public object GetData()
        {
            #if NET_FRAMEWORK
            return CallContext.GetData(id.ToString());
            #elif NET_2_0 || MONO_2_0 || MONO_3_5 || MONO_4_0
            return CallContext.LogicalGetData(id.ToString());
            #else
            AsyncLocal<object> data; if (states.TryGetValue(id, out data))
            {
                return data.Value;
            }
            return data;
            #endif
        }
        /// <summary>
        /// Sets the raw data that should be stored in this local scope
        /// </summary>
        /// <param name="value">An object or null</param>
        public void SetData(object value)
        {
            #if NET_FRAMEWORK
            CallContext.SetData(id.ToString(), value);
            #elif NET_2_0 || MONO_2_0 || MONO_3_5 || MONO_4_0
            CallContext.LogicalSetData(id.ToString(), value);
            #else
            states.GetOrAdd(id, (key) => new AsyncLocal<object>()).Value = value;
            #endif
        }

        /// <summary>
        /// Try to get the underlaying value of this local scope as T if possible
        /// </summary>
        /// <param name="value">Contains the value stored in this local scope or default</param>
        /// <returns>True if a value was found and of type T, false otherwise</returns>
        public bool TryGet(out T value)
        {
            value = default(T);

            object tmp = GetData();
            if (tmp != null && tmp is T)
            {
                value = (T)tmp;
                return true;
            }
            else return false;
        }
    }
}