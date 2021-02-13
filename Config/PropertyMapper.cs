// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using SE.Parsing;

namespace SE.Config
{
    /// <summary>
    /// Manages mapping of data properties to object fields and class properties
    /// </summary>
    public static class PropertyMapper
    {
        private readonly static Type BooleanType = typeof(bool);

        /// <summary>
        /// Applies data properties from the passed provider to certain object instance
        /// </summary>
        /// <param name="target">An object instance to do field and class property lookup</param>
        /// <param name="provider">A property provider to access a set of key-value-pairs</param>
        public static PropertyMapperResult Assign(object target, IPropertyProvider provider, bool ignoreCase = false, bool skipVerbs = false)
        {
            PropertyMapperResult result = new PropertyMapperResult();
            Assign(target.GetType(), target, BindingFlags.Instance, provider, ignoreCase, skipVerbs, ref result);

            return result;
        }
        /// <summary>
        /// Statically applies data properties from the passed provider to fields and class properties
        /// </summary>
        /// <param name="provider">A property provider to access a set of key-value-pairs</param>
        public static PropertyMapperResult Assign<T>(IPropertyProvider provider, bool ignoreCase = false, bool skipVerbs = false)
        {
            PropertyMapperResult result = new PropertyMapperResult();
            Assign(typeof(T), null, BindingFlags.Static, provider, ignoreCase, skipVerbs, ref result);

            return result;
        }

        /// <summary>
        /// Applies data properties from the passed provider to certain object instance including it's base types
        /// </summary>
        /// <param name="target">An object instance to do field and class property lookup</param>
        /// <param name="provider">A property provider to access a set of key-value-pairs</param>
        public static PropertyMapperResult AssignAny(object target, IPropertyProvider provider, bool ignoreCase = false, bool skipVerbs = false)
        {
            PropertyMapperResult result = new PropertyMapperResult();
            Type type = target.GetType();
            do
            {
                Assign(type, target, BindingFlags.Instance, provider, ignoreCase, skipVerbs, ref result);
                type = type.BaseType;
            }
            while (type != null);
            return result;
        }
        /// <summary>
        /// Statically applies data properties from the passed provider to fields and class properties including it's base types
        /// </summary>
        /// <param name="provider">A property provider to access a set of key-value-pairs</param>
        public static PropertyMapperResult AssignAny<T>(IPropertyProvider provider, bool ignoreCase = false, bool skipVerbs = false)
        {
            PropertyMapperResult result = new PropertyMapperResult();
            Type type = typeof(T);
            do
            {
                Assign(type, null, BindingFlags.Static, provider, ignoreCase, skipVerbs, ref result);
                type = type.BaseType;
            }
            while (type != null);
            return result;
        }

        private static void Assign(Type type, object instance, BindingFlags flags, IPropertyProvider provider, bool ignoreCase, bool skipVerbs, ref PropertyMapperResult result)
        {
            Dictionary<MemberInfo, VerbPropertyAttribute> verbs = CollectionPool<Dictionary<MemberInfo, VerbPropertyAttribute>, MemberInfo, VerbPropertyAttribute>.Get();
            result.Parsed = CollectionPool<Dictionary<string, int>, string, int>.Get();
            result.Skipped = CollectionPool<HashSet<string>, string>.Get();
            result.Errors = CollectionPool<List<Exception>, Exception>.Get();
            try
            {
                foreach (MemberInfo memberInfo in type.GetMembers(flags | BindingFlags.Public | BindingFlags.NonPublic))
                    try
                    {
                        Type memberType; if (!memberInfo.TryGetMemberType(out memberType))
                        {
                            continue;
                        }
                        PropertyAttribute[] properties; if (memberInfo.TryGetAttributes<PropertyAttribute>(out properties))
                        {
                            foreach (PropertyAttribute property in properties)
                            {
                                if (property is VerbPropertyAttribute)
                                {
                                    if(!skipVerbs)
                                        verbs.Add(memberInfo, property as VerbPropertyAttribute);
                                }
                                else if (property is NamedPropertyAttribute)
                                {
                                    string name;
                                    int valueCount; if (SetNamedProperty(memberType, memberInfo, instance, property as NamedPropertyAttribute, provider, ignoreCase, out valueCount, out name, ref result))
                                    {
                                        int count; if (result.Parsed.TryGetValue(name, out count))
                                        {
                                            result.Parsed[name] = Math.Max(count, valueCount);
                                        }
                                        else result.Parsed.Add(name, valueCount);
                                        verbs.Remove(memberInfo);
                                        break;
                                    }
                                    else result.Skipped.Add(memberInfo.Name);
                                }
                                else if (property is FlaggedPropertyAttribute)
                                {
                                    if (SetFlaggedProperty(memberType, memberInfo, instance, property as FlaggedPropertyAttribute, provider, ignoreCase))
                                    {
                                        if (!result.Parsed.ContainsKey(memberInfo.Name))
                                        {
                                            result.Parsed.Add(memberInfo.Name, 0);
                                        }
                                        verbs.Remove(memberInfo);
                                        break;
                                    }
                                    else result.Skipped.Add(memberInfo.Name);
                                }
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        result.Errors.Add(er);
                    }
                if (verbs.Count > 0)
                {
                    IEnumerator<PropertyToken> tokens = provider.GetEnumerator();
                    List<KeyValuePair<MemberInfo, VerbPropertyAttribute>> verbProperties = CollectionPool<List<KeyValuePair<MemberInfo, VerbPropertyAttribute>>, KeyValuePair<MemberInfo, VerbPropertyAttribute>>.Get();
                    try
                    {
                        verbProperties.AddRange(verbs.OrderByDescending(x => x.Value.Index));
                        for (int i = verbProperties.Count - 1, verbIndex = 0; verbProperties.Count > 0; i--)
                            try
                            {
                                KeyValuePair<MemberInfo, VerbPropertyAttribute> verbProperty = verbProperties[i];

                                int index = verbProperty.Value.Index;
                                int nextIndex;
                                if (verbProperties.Count > 1)
                                {
                                    nextIndex = verbProperties[i - 1].Value.Index;
                                }
                                else nextIndex = int.MaxValue;
                                bool isCollection = verbProperty.Key.GetUnderlyingType().IsCollection();
                                bool @break = false;
                                bool nextToken;
                                do
                                {
                                    if (!GetNextValueToken(tokens, isCollection, ref result))
                                    {
                                        @break = !isCollection;
                                        break;
                                    }
                                    Type memberType; if (!verbProperty.Key.TryGetMemberType(out memberType))
                                        throw new InvalidOperationException();

                                    if (index == verbIndex)
                                    {
                                        if (SetDefaultProperty(memberType, verbProperty.Key, instance, verbProperty.Value, tokens.Current.Value, out nextToken))
                                        {
                                            verbs.Remove(verbProperty.Key);
                                            result.Verbs++;
                                        }
                                        index++;
                                    }
                                    else nextToken = false;
                                    verbIndex++;
                                }
                                while (!nextToken && index < nextIndex);
                                if (@break)
                                    break;

                                verbProperties.RemoveAt(i);
                            }
                            catch (Exception er)
                            {
                                result.Errors.Add(er);
                            }
                    }
                    finally
                    {
                        CollectionPool<List<KeyValuePair<MemberInfo, VerbPropertyAttribute>>, KeyValuePair<MemberInfo, VerbPropertyAttribute>>.Return(verbProperties);
                    }
                    foreach (KeyValuePair<MemberInfo, VerbPropertyAttribute> defaultProperty in verbs)
                        try
                        {
                            Type memberType; if (!defaultProperty.Key.TryGetMemberType(out memberType))
                                throw new InvalidOperationException();

                            SetDefaultValue(memberType, defaultProperty.Key, instance, defaultProperty.Value);
                            result.Skipped.Add(defaultProperty.Key.Name);
                        }
                        catch (Exception er)
                        {
                            result.Errors.Add(er);
                        }
                }
            }
            finally
            {
                CollectionPool<Dictionary<MemberInfo, VerbPropertyAttribute>, MemberInfo, VerbPropertyAttribute>.Return(verbs);
            }
        }
        private static bool GetNextValueToken(IEnumerator<PropertyToken> tokens, bool stopOnProperty, ref PropertyMapperResult result)
        {
            if (tokens.MoveNext())
            {
                for (; ; )
                {
                    if (tokens.Current.IsValue) return true;
                    else if(stopOnProperty) return false;
                    else
                    {
                        string name = tokens.Current.Value as string;
                        int count; if (result.Parsed.TryGetValue(name, out count))
                        {
                            if (tokens.MoveNext())
                            {
                                if (!tokens.Current.IsValue)
                                    continue;
                            }
                            else return false;
                            for (; count > 0; count--)
                                if (tokens.MoveNext())
                                {
                                    if (!tokens.Current.IsValue)
                                        break;
                                }
                                else return false;

                            result.Parsed[name] = count;
                        }
                        else if (!tokens.MoveNext())
                            return false;
                    }
                }
            }
            else return false;
        }

        private static bool TryGetDefaultValue(Type memberType, PropertyAttribute property, ITypeConverter converter, ref object value)
        {
            if (property.DefaultValue != null && (converter != null || property.DefaultValue.TryCast(memberType, out value)))
            {
                if (converter != null)
                {
                    if (converter.TryParseValue(memberType, property.DefaultValue, out value) || property.DefaultValue.TryCast(memberType, out value))
                    {
                        return true;
                    }
                    else throw new InvalidCastException();
                }
                else return true;
            }
            return false;
        }

        private static bool SetNamedProperty(Type memberType, MemberInfo memberInfo, object instance, NamedPropertyAttribute property, IPropertyProvider provider, bool ignoreCase, out int valueCount, out string name, ref PropertyMapperResult result)
        {
            string[] id = property.Id;
            ITypeConverter converter;
            if (property.TypeConverter != null)
            {
                converter = property.TypeConverter.CreateInstance<ITypeConverter>();
            }
            else converter = null;
            if (id == null || id.Length == 0)
            {
                id = new string[] { memberInfo.Name };
            }
            for (int i = 0; i < id.Length; i++)
            {
                if (ignoreCase)
                {
                    name = id[i].ToLowerInvariant();
                }
                else name = id[i];
                if (memberType.IsCollection())
                {
                    if (HandleCollectionMember(memberType, memberInfo, instance, name, property, provider, converter, i + 1 == id.Length, out valueCount, ref result))
                        return true;
                }
                else if (memberType.IsEnum)
                {
                    if (HandleEnumMember(memberType, memberInfo, instance, name, property, provider, converter, i + 1 == id.Length, out valueCount))
                        return true;
                }
                else if (HandleMember(memberType, memberInfo, instance, name, property, provider, converter, i + 1 == id.Length, out valueCount))
                    return true;
            }
            valueCount = 0;
            name = null;
            return false;
        }
        private static bool HandleCollectionMember(Type memberType, MemberInfo memberInfo, object instance, string name, NamedPropertyAttribute property, IPropertyProvider provider, ITypeConverter converter, bool isLastPossibleMatch, out int valueCount, ref PropertyMapperResult result)
        {
            List<object> values = CollectionPool<List<object>, object>.Get();
            try
            {
                object target; if (memberInfo.MemberType == MemberTypes.Property)
                {
                    target = (memberInfo as PropertyInfo).GetValue(instance, null);
                }
                else target = (memberInfo as FieldInfo).GetValue(instance);
                Type[] genericArgs = memberType.GetGenericArguments();
                while (genericArgs.Length > 1)
                {
                    genericArgs = ArrayExtension.Empty<Type>();
                    foreach (Type @interface in memberType.GetInterfaces())
                        if (@interface.IsCollection())
                        {
                            genericArgs = @interface.GetGenericArguments();
                            memberType = @interface;
                            break;
                        }
                }
                if (genericArgs.Length == 0)
                {
                    valueCount = 0;
                    return false;
                }

                Delegate addItem = memberType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, genericArgs, null).CreateDelegate(target);
                memberType = memberType.GetGenericArguments()[0];

                object defVar = null; if (provider.TryGetValue(property.Cluster, name, values))
                {
                    int counter = 0;
                    valueCount = values.Count;
                    if (converter != null)
                    {
                        foreach (object value in values)
                        {
                            object tmp = value;
                            if (converter.TryParseValue(memberType, value, out defVar) || tmp.TryCast(memberType, out defVar))
                            {
                                Array collection = (defVar as Array);
                                if (collection != null)
                                {
                                    for (int i = 0; i < collection.Length; i++)
                                        try
                                        {
                                            addItem.DynamicInvoke(collection.GetValue(i));
                                        }
                                        catch (Exception er)
                                        {
                                            result.Errors.Add(er);
                                        }
                                }
                                else addItem.DynamicInvoke(defVar);
                                counter++;
                            }
                        }
                    }
                    else foreach (object value in values)
                    {
                        if (value.TryCast(memberType, out defVar))
                        try
                        {
                            addItem.DynamicInvoke(defVar);
                            counter++;
                        }
                        catch (Exception er)
                        {
                            result.Errors.Add(er);
                        }
                    }
                    if (counter > 0)
                    {
                        return true;
                    }
                }
                else valueCount = 0;
                if (isLastPossibleMatch && property.DefaultValue != null && (converter != null || property.DefaultValue.TryCast(memberType, out defVar)))
                {
                    if (converter != null)
                    {
                        if (converter.TryParseValue(memberType, property.DefaultValue, out defVar) || property.DefaultValue.TryCast(memberType, out defVar))
                        {
                            addItem.DynamicInvoke(defVar);
                        }
                        else throw new InvalidCastException();
                    }
                    else addItem.DynamicInvoke(defVar);
                    return true;
                }
            }
            finally
            {
                CollectionPool<List<object>, object>.Return(values);
            }
            return false;
        }
        private static bool HandleEnumMember(Type memberType, MemberInfo memberInfo, object instance, string name, NamedPropertyAttribute property, IPropertyProvider provider, ITypeConverter converter, bool isLastPossibleMatch, out int valueCount)
        {
            object value; if (provider.TryGetValue(property.Cluster, name, out value))
            {
                valueCount = 1;
                if (value is string)
                {
                    try
                    {
                        value = Enum.Parse(memberType, value as string, true);
                        goto Assign;
                    }
                    catch
                    { }
                }
                else if (value.TryCast(typeof(UInt64), out value))
                {
                    try
                    {
                        value = Enum.ToObject(memberType, value);
                        goto Assign;
                    }
                    catch
                    { }
                }
            }
            else valueCount = 0;
            if (isLastPossibleMatch && TryGetDefaultValue(memberType, property, converter, ref value))
            {
                goto Assign;
            }
            return false;

        Assign:

            if (memberInfo.MemberType == MemberTypes.Property)
            {
                (memberInfo as PropertyInfo).SetValue(instance, value, null);
            }
            else (memberInfo as FieldInfo).SetValue(instance, value);
            return true;
        }
        private static bool HandleMember(Type memberType, MemberInfo memberInfo, object instance, string name, NamedPropertyAttribute property, IPropertyProvider provider, ITypeConverter converter, bool isLastPossibleMatch, out int valueCount)
        {
            object value; if (provider.TryGetValue(property.Cluster, name, out value))
            {
                valueCount = 1;
                if (converter != null)
                {
                    object tmp = value;
                    if (converter.TryParseValue(memberType, value, out value) || tmp.TryCast(memberType, out value))
                    {
                        goto Assign;
                    }
                }
                else if(value.TryCast(memberType, out value))
                    goto Assign;
            }
            valueCount = 0;
            if (memberType == BooleanType && property.DefaultValue == null)
            {
                value = provider.ContainsKey(property.Cluster, name);
                if((bool)value)
                    goto Assign;
            }
            if (isLastPossibleMatch && TryGetDefaultValue(memberType, property, converter, ref value))
            {
                goto Assign;
            }
            return false;

        Assign:

            if (memberInfo.MemberType == MemberTypes.Property)
            {
                (memberInfo as PropertyInfo).SetValue(instance, value, null);
            }
            else (memberInfo as FieldInfo).SetValue(instance, value);
            return true;
        }

        private static bool SetFlaggedProperty(Type memberType, MemberInfo memberInfo, object instance, FlaggedPropertyAttribute property, IPropertyProvider provider, bool ignoreCase)
        {
            if (memberType.IsEnum)
            {
                ITypeConverter converter; if (property.TypeConverter != null)
                {
                    converter = property.TypeConverter.CreateInstance<ITypeConverter>();
                }
                else converter = null;
                bool parsed = false;

                UInt64 result = 0;
                Array values = Enum.GetValues(memberType);
                string[] ids = Enum.GetNames(memberType);
                for(int i = 0; i < ids.Length; i++)
                {
                    string name; if (ignoreCase)
                    {
                        name = ids[i].ToLowerInvariant();
                    }
                    else name = ids[i];
                    if (provider.ContainsKey(property.Cluster, name))
                    {
                        if (!parsed)
                            parsed = true;

                        result |= Convert.ToUInt64(values.GetValue(i));
                    }
                }
                object value = result;
                if (!parsed)
                {
                    if (property.DefaultValue != null && (converter != null || property.DefaultValue.TryCast(memberType, out value)))
                    {
                        if (converter != null)
                        {
                            if (converter.TryParseValue(memberType, property.DefaultValue, out value) || property.DefaultValue.TryCast(memberType, out value))
                            {
                                goto Assign;
                            }
                            else throw new InvalidCastException();
                        }
                        else goto Assign;
                    }
                    return false;
                }

            Assign:

                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    (memberInfo as PropertyInfo).SetValue(instance, Enum.ToObject(memberType, value), null);
                }
                else (memberInfo as FieldInfo).SetValue(instance, Enum.ToObject(memberType, value));
                return true;
            }
            else return false;
        }

        private static bool SetDefaultProperty(Type memberType, MemberInfo memberInfo, object instance, VerbPropertyAttribute property, object value, out bool next)
        {
            ITypeConverter converter; if (property.TypeConverter != null)
            {
                converter = property.TypeConverter.CreateInstance<ITypeConverter>();
            }
            else converter = null;
            if (memberType.IsCollection())
            {
                next = false;
                if (HandleCollectionMember(memberType, memberInfo, instance, property, converter, value))
                    return true;
            }
            else
            {
                next = true;
                if (memberType.IsEnum)
                {
                    if (HandleEnumMember(memberType, memberInfo, instance, property, converter, value))
                        return true;
                }
                else if (HandleMember(memberType, memberInfo, instance, property, converter, value))
                    return true;
            }
            return false;
        }
        private static bool HandleCollectionMember(Type memberType, MemberInfo memberInfo, object instance, VerbPropertyAttribute property, ITypeConverter converter, object value)
        {
            object target; if (memberInfo.MemberType == MemberTypes.Property)
            {
                target = (memberInfo as PropertyInfo).GetValue(instance, null);
            }
            else target = (memberInfo as FieldInfo).GetValue(instance);

            Delegate addItem = memberType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, memberType.GetGenericArguments(), null).CreateDelegate(target);
            memberType = memberType.GetGenericArguments()[0];

            if (converter != null)
            {
                object tmp = value;
                if (converter.TryParseValue(memberType, value, out value) || tmp.TryCast(memberType, out value))
                {
                    addItem.DynamicInvoke(value);
                }
            }
            else if(value.TryCast(memberType, out value))
                addItem.DynamicInvoke(value);

            return true;
        }
        private static bool HandleEnumMember(Type memberType, MemberInfo memberInfo, object instance, VerbPropertyAttribute property, ITypeConverter converter, object value)
        {
            if (value is string)
            {
                try
                {
                    value = Enum.Parse(memberType, value as string, true);
                    goto Assign;
                }
                catch
                { }
            }
            else if (value.TryCast(typeof(UInt64), out value))
            {
                try
                {
                    value = Enum.ToObject(memberType, value);
                    goto Assign;
                }
                catch
                { }
            }

            if (TryGetDefaultValue(memberType, property, converter, ref value))
            {
                goto Assign;
            }
            return false;

        Assign:

            if (memberInfo.MemberType == MemberTypes.Property)
            {
                (memberInfo as PropertyInfo).SetValue(instance, value, null);
            }
            else (memberInfo as FieldInfo).SetValue(instance, value);
            return true;
        }
        private static bool HandleMember(Type memberType, MemberInfo memberInfo, object instance, VerbPropertyAttribute property, ITypeConverter converter, object value)
        {
            if (converter != null)
            {
                object tmp = value;
                if (converter.TryParseValue(memberType, value, out value) || tmp.TryCast(memberType, out value))
                {
                    goto Assign;
                }
            }
            else if (value.TryCast(memberType, out value))
                goto Assign;

            if (TryGetDefaultValue(memberType, property, converter, ref value))
            {
                goto Assign;
            }
            return false;

        Assign:

            if (memberInfo.MemberType == MemberTypes.Property)
            {
                (memberInfo as PropertyInfo).SetValue(instance, value, null);
            }
            else (memberInfo as FieldInfo).SetValue(instance, value);
            return true;
        }

        private static void SetDefaultValue(Type memberType, MemberInfo memberInfo, object instance, VerbPropertyAttribute property)
        {
            if (!memberType.IsCollection())
            {
                object value = null;
                ITypeConverter converter;
                if (property.TypeConverter != null)
                {
                    converter = property.TypeConverter.CreateInstance<ITypeConverter>();
                }
                else converter = null;
                if (TryGetDefaultValue(memberType, property, converter, ref value))
                {
                    if (memberInfo.MemberType == MemberTypes.Property)
                    {
                        (memberInfo as PropertyInfo).SetValue(instance, value, null);
                    }
                    else (memberInfo as FieldInfo).SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// Collects provided information on fields and class properties
        /// </summary>
        /// <param name="target">An object instance to do field and class property lookup</param>
        public static void GetPropertyDescriptions(object target, PropertyPage page, bool ignoreCase = false, bool skipPropertiesWithoutDescription = true)
        {
            Type type = target.GetType();
            do
            {
                GetPropertyDescriptions(type, BindingFlags.Instance, page, ignoreCase, skipPropertiesWithoutDescription);
                type = type.BaseType;
            }
            while (type != null);
        }
        /// <summary>
        /// Collects provided information on fields and class properties
        /// </summary>
        public static void GetPropertyDescriptions<T>(PropertyPage page, bool ignoreCase = false, bool skipPropertiesWithoutDescription = true)
        {
            Type type = typeof(T);
            do
            {
                GetPropertyDescriptions(type, BindingFlags.Static, page, ignoreCase, skipPropertiesWithoutDescription);
                type = type.BaseType;
            }
            while (type != null);
        }
        /// <summary>
        /// Collects provided information on fields and class properties
        /// </summary>
        public static void GetPropertyDescriptions(Type type, BindingFlags flags, PropertyPage page, bool ignoreCase, bool skipPropertiesWithoutDescription)
        {
            foreach (MemberInfo memberInfo in type.GetMembers(flags | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Type memberType; if (!memberInfo.TryGetMemberType(out memberType))
                {
                    continue;
                }
                PropertyAttribute[] properties; if (memberInfo.TryGetAttributes<PropertyAttribute>(out properties))
                {
                    List<string> keys = CollectionPool<List<string>, string>.Get();
                    try
                    {
                        int verbIndex = 0;
                        string defaultValue = string.Empty;
                        PropertyDescriptionAttribute description = memberInfo.GetAttribute<PropertyDescriptionAttribute>();
                        if (description == null && skipPropertiesWithoutDescription)
                        {
                            continue;
                        }
                        foreach (PropertyAttribute property in properties)
                        {
                            if (property is VerbPropertyAttribute)
                            {
                                if (ignoreCase)
                                {
                                    keys.Insert(0, memberInfo.Name.ToLowerInvariant());
                                }
                                else keys.Insert(0, memberInfo.Name);
                                verbIndex = (property as VerbPropertyAttribute).Index;
                            }
                            else if (property is NamedPropertyAttribute)
                            {
                                string[] id = (property as NamedPropertyAttribute).Id;
                                ITypeConverter converter;
                                if (property.TypeConverter != null)
                                {
                                    converter = property.TypeConverter.CreateInstance<ITypeConverter>();
                                }
                                else converter = null;
                                if (id.Length == 0)
                                {
                                    id = new string[] { memberInfo.Name };
                                }
                                StringBuilder key = new StringBuilder();
                                for (int i = 0; i < id.Length; i++)
                                {
                                    if (key.Length > 0)
                                    {
                                        key.Append(", ");
                                    }
                                    if (ignoreCase)
                                    {
                                        key.Append(page.MakeKey(id[i].ToLowerInvariant()));
                                    }
                                    else key.Append(page.MakeKey(id[i]));
                                }
                                keys.Add(key.ToString());
                            }
                            else if (property is FlaggedPropertyAttribute)
                            {
                                if (memberType.IsEnum)
                                {
                                    bool lowerCase = ignoreCase && (page.Flags & PropertyPageFlags.HarmonizeFlags) != PropertyPageFlags.HarmonizeFlags;
                                    foreach (FieldInfo field in memberType.GetFields())
                                        if (field.FieldType == memberType)
                                        {
                                            PropertyDescriptionAttribute flagDescriptor = field.GetAttribute<PropertyDescriptionAttribute>();
                                            if (lowerCase)
                                            {
                                                page.AddRow(page.MakeKey(field.Name.ToLowerInvariant()), flagDescriptor);
                                            }
                                            else page.AddRow(page.MakeKey(field.Name), flagDescriptor);
                                        }
                                }
                            }
                            if (property.DefaultValue != null)
                            {
                                defaultValue = property.DefaultValue.ToString();
                            }
                        }

                        if (keys.Count > 0)
                        {
                            PropertyPageKeyValueRow row = page.AddRow(keys[0], description);
                            row.DefaultValue = defaultValue;
                            row.VerbIndex = verbIndex;
                        }
                        for (int i = 1; i < keys.Count; i++)
                        {
                            page.AppendKey(keys[i]);
                        }
                    }
                    finally
                    {
                        CollectionPool<List<string>, string>.Return(keys);
                    }
                }
            }
        }
    }
}
