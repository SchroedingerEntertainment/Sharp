// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Parsing
{
    /// <summary>
    /// Defines a generic access interface to parsed data stores
    /// </summary>
    public interface IPropertyProvider : IEnumerable<PropertyToken>
    {
        /// <summary>
        /// Determines if the given key is present in this provider
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <returns>True if the key is set, false otherwise</returns>
        bool ContainsKey(string category, string key);

        /// <summary>
        /// Tries to obtain a value from a given key
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <param name="value">A value instance returned on success</param>
        /// <param name="defaultValue">A default value assigned on failure</param>
        /// <returns>True if the key exists and has a value attached to it, false otherwise</returns>
        bool TryGetValue(string category, string key, out object value, object defaultValue = null);
        /// <summary>
        /// Tries to obtain a value from a given key
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <param name="value">A value instance to be filled</param>
        /// <param name="defaultValue">A default value assigned on failure</param>
        /// <returns>True if the key exists and has a value attached to it, false otherwise</returns>
        bool TryGetValue(string category, string key, ICollection<object> value);
        /// <summary>
        /// Tries to obtain a value from a given key
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <param name="value">A value instance returned on success</param>
        /// <param name="defaultValue">A default value assigned on failure</param>
        /// <returns>True if the key exists and has a value attached to it, false otherwise</returns>
        bool TryGetValue(string category, string key, out string value, string defaultValue = null);
        /// <summary>
        /// Tries to obtain a value from a given key
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <param name="value">A value instance to be filled</param>
        /// <param name="defaultValue">A default value assigned on failure</param>
        /// <returns>True if the key exists and has a value attached to it, false otherwise</returns>
        bool TryGetValue(string category, string key, ICollection<string> value);
        /// <summary>
        /// Tries to obtain a value from a given key
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <param name="value">A value instance returned on success</param>
        /// <param name="defaultValue">A default value assigned on failure</param>
        /// <returns>True if the key exists and has a value attached to it that could be converted to T, false otherwise</returns>
        bool TryGetValue<T>(string category, string key, out T value, T defaultValue = default(T));
        /// <summary>
        /// Tries to obtain a value from a given key
        /// </summary>
        /// <param name="category">An optional id scope to search for</param>
        /// <param name="value">A value instance to be filled</param>
        /// <param name="defaultValue">A default value assigned on failure</param>
        /// <returns>True if the key exists and has a value attached to it that could be converted to T, false otherwise</returns>
        bool TryGetValue<T>(string category, string key, ICollection<T> value);
    }
}