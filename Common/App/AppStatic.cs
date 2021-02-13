// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;

namespace System
{
    /// <summary>
    /// 
    /// </summary>
    public class AppStatic<T> : FinalizerObject where T : MarshalByRefObject, new()
    {
        class ItemContainer : AppDomainʾ.ReferenceObject
        {
            T data;
            public T Value
            {
                get { return data; }
                set { data = value; }
            }

            public ItemContainer()
            {}
        }
        private readonly static Type ItemContainerType = typeof(ItemContainer);

        ItemContainer instance;
        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get { return instance.Value; }
            set { instance.Value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasValue
        {
            get { return (instance.Value != null); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public AppStatic(string name)
        {
            AppDomain domain = AppDomainʾ.DefaultDomain;
            lock (domain)
            {
                instance = (domain.GetData(name) as ItemContainer);
                if (instance == null)
                {
                    ObjectHandle handle = Activator.CreateInstance(domain, Assembly.GetExecutingAssembly().FullName, ItemContainerType.FullName);
                    instance = (handle.Unwrap() as ItemContainer);

                    domain.SetData(name, instance);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public AppStatic()
         : this(ItemContainerType.FullName)
        { }
        protected override void Dispose(bool disposing)
        {
            instance.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CreateValue()
        {
            lock (instance)
            {
                if (!HasValue)
                {
                    Type itemType = typeof(T);

                    ObjectHandle handle = Activator.CreateInstance(AppDomainʾ.DefaultDomain, itemType.Assembly.FullName, itemType.FullName);
                    instance.Value = (handle.Unwrap() as T);

                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(out T value)
        {
            value = instance.Value;
            return (value != null);
        }
    }
}
#endif