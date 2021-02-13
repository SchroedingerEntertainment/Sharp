// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using SE.Parsing;

namespace SE.Json
{
    public partial class JsonDocument : IPropertyProvider
    {
        /// <summary>
        /// A class specific allocation free enumerator
        /// </summary>
        public struct Enumerator : IEnumerator<PropertyToken>
        {
            JsonNode root;
            bool value;

            PropertyToken current;

            public PropertyToken Current
            {
                get { return current; }
            }
            object IEnumerator.Current
            {
                get { return current; }
            }

            public Enumerator(JsonNode root)
            {
                if (root != null)
                {
                    root = root.Child;
                }
                this.root = root;
                this.current = new PropertyToken();
                this.value = false;
            }
            public void Dispose()
            {
                root = null;
            }

            public bool MoveNext()
            {

            Head:
                if (root != null)
                {
                    if (!value)
                    {
                        string name = root.Name;
                        if (name == null)
                        {
                            name = string.Empty;
                        }
                        current = new PropertyToken(false, root.Name);
                        value = true;
                    }
                    else switch (root.Type)
                        {
                            case JsonNodeType.Array:
                            case JsonNodeType.Object:
                                {
                                    root = root.Next;
                                    value = false;
                                }
                                goto Head;
                            default:
                                {
                                    string content = root.ToString();
                                    if (content == null)
                                    {
                                        content = string.Empty;
                                    }
                                    current = new PropertyToken(true, content);
                                    root = root.Next;
                                    value = false;
                                }
                                break;
                        }
                    return true;
                }
                else return false;
            }
            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public bool ContainsKey(string category, string key)
        {
            object value; if (TryGetValue(category, key, out value))
            {
                return true;
            }
            else return false;
        }
        public bool ContainsKey(string key)
        {
            return ContainsKey(null, key);
        }

        public bool TryGetValue(string category, string key, out object value, object defaultValue = null)
        {
            Stack<string> path = StackPool<string>.Get();
            GetPath(category, path);
            try
            {
                JsonNode node = GetFirstChild();
                while (node != null)
                {
                    if (path.Count == 0)
                    {
                        if (key.Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            switch (node.Type)
                            {
                                case JsonNodeType.Array:
                                case JsonNodeType.Object:
                                    {
                                        value = node;
                                    }
                                    break;
                                default:
                                    {
                                        value = node.RawValue;
                                    }
                                    break;
                                case JsonNodeType.Empty:
                                case JsonNodeType.Undefined:
                                    {
                                        value = defaultValue;
                                    }
                                    break;
                            }
                            return true;
                        }
                    }
                    else if (path.Peek().Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        node = node.Child;
                        path.Pop();

                        continue;
                    }
                    node = node.Next;
                }
                value = defaultValue;
                return false;
            }
            finally
            {
                StackPool<string>.Return(path);
            }
        }
        public bool TryGetValue(string category, string key, ICollection<object> value)
        {
            Stack<string> path = StackPool<string>.Get();
            GetPath(category, path);
            try
            {
                JsonNode node = GetFirstChild();
                while (node != null)
                {
                    if (path.Count == 0)
                    {
                        if (key.Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            switch (node.Type)
                            {
                                case JsonNodeType.Array:
                                    {
                                        FillFromArray(node, value);
                                    }
                                    break;
                                case JsonNodeType.Object:
                                    {
                                        value.Add(node);
                                    }
                                    break;
                                default:
                                    {
                                        value.Add(node.RawValue);
                                    }
                                    break;
                                case JsonNodeType.Empty:
                                case JsonNodeType.Undefined:
                                    break;
                            }
                        }
                    }
                    else if (path.Peek().Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        node = node.Child;
                        path.Pop();

                        continue;
                    }
                    node = node.Next;
                }
                return (value.Count > 0);
            }
            finally
            {
                StackPool<string>.Return(path);
            }
        }
        public bool TryGetValue(string key, out object value, object defaultValue = null)
        {
            return TryGetValue(null, key, out value, defaultValue);
        }
        public bool TryGetValue(string key, ICollection<object> value)
        {
            return TryGetValue(null, key, value);
        }
        private static void FillFromArray(JsonNode node, ICollection<object> value)
        {
            node = node.Child;
            while (node != null)
            {
                switch (node.Type)
                {
                    case JsonNodeType.Array:
                    case JsonNodeType.Object:
                        {
                            value.Add(node);
                        }
                        break;
                    default:
                        {
                            value.Add(node.RawValue);
                        }
                        break;
                    case JsonNodeType.Empty:
                    case JsonNodeType.Undefined:
                        break;
                }
                node = node.Next;
            }
        }

        public bool TryGetValue(string category, string key, out string value, string defaultValue = null)
        {
            object result; if (!TryGetValue(category, key, out result, defaultValue))
            {
                if (string.IsNullOrEmpty(defaultValue))
                {
                    value = string.Empty;
                }
                else value = defaultValue;
                return false;
            }
            else
            {
                if (result == null)
                {
                    result = string.Empty;
                }
                value = result.ToString();
                return true;
            }
        }
        public bool TryGetValue(string category, string key, ICollection<string> value)
        {
            Stack<string> path = StackPool<string>.Get();
            GetPath(category, path);
            try
            {
                JsonNode node = GetFirstChild();
                while (node != null)
                {
                    if (path.Count == 0)
                    {
                        if (key.Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            switch (node.Type)
                            {
                                case JsonNodeType.Array:
                                    {
                                        FillFromArray(node, value);
                                    }
                                    break;
                                case JsonNodeType.Empty:
                                    {
                                        value.Add(string.Empty);
                                    }
                                    break;
                                default:
                                    {
                                        value.Add(node.ToString());
                                    }
                                    break;
                                case JsonNodeType.Object:
                                case JsonNodeType.Undefined:
                                    break;
                            }
                        }
                    }
                    else if (path.Peek().Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        node = node.Child;
                        path.Pop();

                        continue;
                    }
                    node = node.Next;
                }
                return (value.Count > 0);
            }
            finally
            {
                StackPool<string>.Return(path);
            }
        }
        public bool TryGetValue(string key, out string value, string defaultValue = null)
        {
            return TryGetValue(null, key, out value, defaultValue);
        }
        public bool TryGetValue(string key, ICollection<string> value)
        {
            return TryGetValue(null, key, value);
        }
        private static void FillFromArray(JsonNode node, ICollection<string> value)
        {
            node = node.Child;
            while (node != null)
            {
                switch (node.Type)
                {
                    case JsonNodeType.Empty:
                        {
                            value.Add(string.Empty);
                        }
                        break;
                    default:
                        {
                            value.Add(node.ToString());
                        }
                        break;
                    case JsonNodeType.Array:
                    case JsonNodeType.Object:
                    case JsonNodeType.Undefined:
                        break;
                }
                node = node.Next;
            }
        }

        public bool TryGetValue<T>(string category, string key, out T value, T defaultValue = default(T))
        {
            object result; if (!TryGetValue(category, key, out result, defaultValue))
            {
                value = defaultValue;
                return false;
            }
            else return result.TryCast<T>(out value);
        }
        public bool TryGetValue<T>(string category, string key, ICollection<T> value)
        {
            Stack<string> path = StackPool<string>.Get();
            GetPath(category, path);
            try
            {
                JsonNode node = GetFirstChild();
                while (node != null)
                {
                    if (path.Count == 0)
                    {
                        T tmp; if (key.Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            switch (node.Type)
                            {
                                case JsonNodeType.Array:
                                    {
                                        FillFromArray(node, value);
                                    }
                                    break;
                                case JsonNodeType.Object: if (node.TryCast<T>(out tmp))
                                    {
                                        value.Add(tmp);
                                    }
                                    break;
                                default: if(node.RawValue.TryCast<T>(out tmp))
                                    {
                                        value.Add(tmp);
                                    }
                                    break;
                                case JsonNodeType.Empty:
                                case JsonNodeType.Undefined:
                                    break;
                            }
                        }
                    }
                    else if (path.Peek().Equals(node.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        node = node.Child;
                        path.Pop();

                        continue;
                    }
                    node = node.Next;
                }
                return (value.Count > 0);
            }
            finally
            {
                StackPool<string>.Return(path);
            }
        }
        public bool TryGetValue<T>(string key, out T value, T defaultValue = default(T))
        {
            return TryGetValue<T>(null, key, out value, defaultValue);
        }
        public bool TryGetValue<T>(string key, ICollection<T> value)
        {
            return TryGetValue<T>(null, key, value);
        }
        private static void FillFromArray<T>(JsonNode node, ICollection<T> value)
        {
            node = node.Child;
            T tmp; while (node != null)
            {
                switch (node.Type)
                {
                    case JsonNodeType.Array:
                    case JsonNodeType.Object: if (node.TryCast<T>(out tmp))
                        {
                            value.Add(tmp);
                        }
                        break;
                    default: if (node.RawValue.TryCast<T>(out tmp))
                        {
                            value.Add(tmp);
                        }
                        break;
                    case JsonNodeType.Empty:
                    case JsonNodeType.Undefined:
                        break;
                }
                node = node.Next;
            }
        }

        void GetPath(string category, Stack<string> path)
        {
            if (category != null)
            {
                foreach (string stage in category.Split('/').Reverse())
                    path.Push(stage);
            }
        }
        JsonNode GetFirstChild()
        {
            JsonNode root = Root;
            if (root != null)
            {
                root = root.Child;
            }
            return root;
        }

        public IEnumerator<PropertyToken> GetEnumerator()
        {
            return new Enumerator(Root);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
