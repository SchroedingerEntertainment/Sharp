// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using SE.Parsing;

namespace SE.Alchemy
{
    public partial class PropertySheet : IPropertyProvider
    {
        private readonly static IEnumerable<PropertyToken> Empty = new PropertyToken[0];

        public bool ContainsKey(string category, string key)
        {
            if (propertyModule != null)
            {
                return propertyModule.ContainsKey(category, key);
            }
            else return false;
        }
        public bool ContainsKey(string key)
        {
            return ContainsKey(string.Empty, key);
        }

        public bool TryGetValue(string category, string key, out object value, object defaultValue = null)
        {
            if (propertyModule != null)
            {
                return propertyModule.TryGetValue(category, key, out value, defaultValue);
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }
        public bool TryGetValue(string category, string key, ICollection<object> value)
        {
            if (propertyModule != null)
            {
                return propertyModule.TryGetValue(category, key, value);
            }
            else return false;
        }
        public bool TryGetValue(string key, out object value, object defaultValue = null)
        {
            return TryGetValue(string.Empty, key, out value, defaultValue);
        }
        public bool TryGetValue(string key, ICollection<object> value)
        {
            return TryGetValue(string.Empty, key, value);
        }

        public bool TryGetValue(string category, string key, out string value, string defaultValue = null)
        {
            if (propertyModule != null)
            {
                return propertyModule.TryGetValue(category, key, out value, defaultValue);
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }
        public bool TryGetValue(string category, string key, ICollection<string> value)
        {
            if (propertyModule != null)
            {
                return propertyModule.TryGetValue(category, key, value);
            }
            else return false;
        }
        public bool TryGetValue(string key, out string value, string defaultValue = null)
        {
            return TryGetValue(string.Empty, key, out value, defaultValue);
        }
        public bool TryGetValue(string key, ICollection<string> value)
        {
            return TryGetValue(string.Empty, key, value);
        }

        public bool TryGetValue<T>(string category, string key, out T value, T defaultValue = default(T))
        {
            if (propertyModule != null)
            {
                return propertyModule.TryGetValue<T>(category, key, out value, defaultValue);
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }
        public bool TryGetValue<T>(string category, string key, ICollection<T> value)
        {
            if (propertyModule != null)
            {
                return propertyModule.TryGetValue<T>(category, key, value);
            }
            else return false;
        }
        public bool TryGetValue<T>(string key, out T value, T defaultValue = default(T))
        {
            return TryGetValue<T>(string.Empty, key, out value, defaultValue);
        }
        public bool TryGetValue<T>(string key, ICollection<T> value)
        {
            return TryGetValue<T>(string.Empty, key, value);
        }

        public IEnumerator<PropertyToken> GetEnumerator()
        {
            if (propertyModule != null)
            {
                return propertyModule.GetEnumerator();
            }
            else return Empty.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
