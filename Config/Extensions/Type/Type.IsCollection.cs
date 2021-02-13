// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;

namespace SE.Config
{
    public static partial class TypeExtension
    {
        private readonly static Type CollectionType = typeof(ICollection<>);

        /// <summary>
        /// Determines if the type implements the generic ICollection<> interface
        /// </summary>
        public static bool IsCollection(this Type type)
        {
            return (type.IsGenericType && CollectionType.IsAssignableFrom(type.GetGenericTypeDefinition()) || type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == CollectionType));
        }
    }
}
