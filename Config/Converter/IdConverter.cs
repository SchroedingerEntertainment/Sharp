// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Config
{
    /// <summary>
    /// Converts string or number values into UInt32 IDs
    /// </summary>
    public class IdConverter : ITypeConverter
    {
        private readonly static Type IdType = typeof(UInt32);

        /// <summary>
        /// Creates a new instance of this converter
        /// </summary>
        public IdConverter()
        { }

        public bool TryParseValue(Type targetType, object value, out object result)
        {
            if (targetType != IdType)
            {
                goto Failure;
            }
            if (value is string)
            {
                result = value.ToString().Fnv32();
                return true;
            }
            else
            {
                UInt32 id; if (value.TryCast<UInt32>(out id))
                {
                    result = id;
                    return true;
                }
                else goto Failure;
            }

        Failure:
            result = null;
            return false;
        }
    }
}
