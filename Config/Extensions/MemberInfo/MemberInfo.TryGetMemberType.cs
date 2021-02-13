// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SE.Config
{
    public static partial class MemberInfoExtension
    {
        /// <summary>
        /// Determines the underlaying MemberType of a field or class property
        /// </summary>
        /// <returns>True if MemberInfo is a field or class property info, false otherwise</returns>
        public static bool TryGetMemberType(this MemberInfo memberInfo, out Type memberType)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    {
                        memberType = (memberInfo as PropertyInfo).PropertyType;
                        return true;
                    }
                case MemberTypes.Field:
                    {
                        memberType = (memberInfo as FieldInfo).FieldType;
                        return true;
                    }
                default:
                    {
                        memberType = null;
                        return false;
                    }
            }
        }
    }
}
