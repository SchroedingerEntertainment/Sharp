// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System
{
    public static partial class ArrayExtension
    {
        #if net40 || net403 || net45 || net451 || net452
        /// <summary>
        /// 
        /// </summary>
        class EmptyArray<T>
        {
            static T[] instance;
            /// <summary>
            /// 
            /// </summary>
            public static T[] Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new T[0];
                    }
                    return instance;
                }
            }

            private EmptyArray()
            { }
        }
        #endif

        /// <summary>
        /// Obtains a reference to the default empty Array
        /// </summary>
        public static T[] Empty<T>()
        {
            #if net40 || net403 || net45 || net451 || net452
            return EmptyArray<T>.Instance;
            #else
            return Array.Empty<T>();
            #endif
        }
    }
}