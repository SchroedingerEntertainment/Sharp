// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using SE.Parsing;

namespace SE.CommandLine
{
    public partial class CommandLineOptions : IPropertyProvider
    {
        /// <summary>
        /// A class specific allocation free enumerator
        /// </summary>
        public struct Enumerator : IEnumerator<PropertyToken>
        {
            CommandLineToken root;
            PropertyToken current;

            public PropertyToken Current
            {
                get { return current; }
            }
            object IEnumerator.Current
            {
                get { return current; }
            }

            public Enumerator(CommandLineToken root)
            {
                this.root = root;
                this.current = new PropertyToken();
            }
            public void Dispose()
            {
                root = null;
            }

            public bool MoveNext()
            {
                while (root != null)
                {
                    switch (root.Type)
                    {
                        case CommandLineTokenType.ParserGenerated:
                        case CommandLineTokenType.PlainText:
                            break;
                        default:
                            {
                                current = new PropertyToken(root.IsValue, root.Value);
                                root = root.Next;
                                return true;
                            }
                    }
                    root = root.Next;
                }
                return false;
            }
            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        public bool ContainsKey(string category, string key)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return properties.ContainsKey(key);
            }
            else return false;
        }
        public bool ContainsKey(string key)
        {
            return ContainsKey(string.Empty, key);
        }

        public bool TryGetValue(string category, string key, out object value, object defaultValue = null)
        {
            value = defaultValue;
            if (!string.IsNullOrWhiteSpace(category))
                return false;

            HashSet<CommandLineToken> tokens; if (properties.TryGetValue(key, out tokens))
            {
                CommandLineToken token = tokens.FirstOrDefault();
                if (token != null)
                {
                    token = token.Next;
                    if (token != null && token.IsValue)
                    {
                        value = token.Value;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool TryGetValue(string category, string key, ICollection<object> value)
        {
            if (!string.IsNullOrWhiteSpace(category))
                return false;

            HashSet<CommandLineToken> tokens; if (properties.TryGetValue(key, out tokens))
            {
                foreach (CommandLineToken t in tokens)
                {
                    CommandLineToken token = t.Next;
                    while (token != null && token.IsValue)
                    {
                        value.Add(token.Value);
                        token = token.Next;
                    }
                }
            }
            return (value.Count > 0);
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
            object result; if (!string.IsNullOrWhiteSpace(category) || !TryGetValue(key, out result, defaultValue))
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
            if (!string.IsNullOrWhiteSpace(category))
                return false;

            HashSet<CommandLineToken> tokens; if (properties.TryGetValue(key, out tokens))
            {
                foreach (CommandLineToken t in tokens)
                {
                    CommandLineToken token = t.Next;
                    while (token != null && token.IsValue)
                    {
                        value.Add(token.ToString());
                        token = token.Next;
                    }
                }
            }
            return (value.Count > 0);
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
            object result; if (!TryGetValue(key, out result, defaultValue))
            {
                value = defaultValue;
                return false;
            }
            else return result.TryCast<T>(out value);
        }
        public bool TryGetValue<T>(string category, string key, ICollection<T> value)
        {
            if (!string.IsNullOrWhiteSpace(category))
                return false;

            HashSet<CommandLineToken> tokens; if (properties.TryGetValue(key, out tokens))
            {
                T tmp; foreach(CommandLineToken t in tokens)
                {
                    CommandLineToken token = t.Next;
                    while (token != null && token.IsValue)
                    {
                        if (token.TryCast<T>(out tmp))
                        {
                            value.Add(tmp);
                        }
                        token = token.Next;
                    }
                }
            }
            return (value.Count > 0);
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
            return new Enumerator(First);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
