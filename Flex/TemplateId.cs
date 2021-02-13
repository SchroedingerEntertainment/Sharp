// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Threading;

namespace SE.Flex
{
    /// <summary>
    /// A 64 bit ID used to identify an object and/or a component
    /// </summary>
    [Serializable]
    public struct TemplateId
    {
        public readonly static TemplateId Invalid = new TemplateId();
        
        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472
        class IdContainer : AppDomainʾ.ReferenceObject
        {
            atomic_int value;

            public IdContainer()
            { }

            public Int32 Increment()
            {
                return value.Increment();
            }
        }
        private static AppStatic<IdContainer> nextId;
        #else
        private static atomic_int nextId;
        #endif

        private readonly UInt64 value;

        /// <summary>
        /// The underlaying object ID
        /// </summary>
        public UInt32 ObjectId
        {
            get { return (UInt32)(value >> 32); }
        }
        /// <summary>
        /// An optional component ID
        /// </summary>
        public UInt32 ComponentId
        {
            get { return (UInt32)(value & UInt32.MaxValue); }
        }

        #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472
        static TemplateId()
        {
            nextId = new AppStatic<IdContainer>();
            nextId.CreateValue();
        }
        #endif
        /// <summary>
        /// Creates a new instance ID from the given integer
        /// </summary>
        /// <param name="id"></param>
        public TemplateId(UInt64 id)
        {
            this.value = id;
        }
        /// <summary>
        /// Creates a new instance ID from the given integer
        /// </summary>
        /// <param name="id"></param>
        public TemplateId(UInt32 id)
        {
            this.value = (((UInt64)id) << 32);
        }

        public static TemplateId operator |(TemplateId lhs, UInt32 rhs)
        {
            return new TemplateId(((UInt64)lhs.ObjectId << 32) | rhs);
        }

        public static implicit operator UInt64(TemplateId id)
        {
            return id.value;
        }
        public static implicit operator TemplateId(UInt32 id)
        {
            return new TemplateId(id);
        }
        public static implicit operator TemplateId(UInt64 id)
        {
            return new TemplateId(id);
        }

        public int CompareTo(TemplateId other)
        {
            return value.CompareTo(other.value);
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("Object: {0}, Component: {1}", ObjectId, ComponentId);
        }

        /// <summary>
        /// Creates a new semi unique ID
        /// </summary>
        /// <remarks>
        /// A 32 bit ID is unique up to the point when 2^32-1 IDs have been created.
        /// The ID will then swap over to start again from 1 (0 is invalid)
        /// </remarks>
        public static TemplateId Create()
        {
            int id;
            do
            {
                #if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472
                id = nextId.Value.Increment();
                #else
                id = nextId.Increment();
                #endif
            }
            while (id == 0);
            return new TemplateId((UInt32)id);
        }
    }
}
