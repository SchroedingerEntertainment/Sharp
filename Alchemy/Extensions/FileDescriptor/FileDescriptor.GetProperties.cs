// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime;
using SE.Parsing;

namespace SE.Alchemy
{
    public static partial class FileDescriptorExtensions
    {
        /// <summary>
        /// Loads a property provider from this file's content if possible
        /// </summary>
        /// <param name="defines">A collection of defined constants to be processed by Alchemy</param>
        /// <param name="logger">A logging interface to write parser errors to</param>
        /// <param name="result">The resulting property provider if processing succeeded</param>
        /// <returns>True if the file has successfully been processed, false otherwise</returns>
        public static bool GetProperties(this FileDescriptor file, IEnumerable<KeyValuePair<string, string>> defines, ILogSystem logger, out IPropertyProvider result)
        {
            switch (file.Extension.ToLowerInvariant())
            {
                case "alc":
                case "alch":
                case "arc":
                    {
                        PropertySheet properties = new PropertySheet(file);
                        foreach (KeyValuePair<string, string> define in defines)
                        {
                            properties.Defines.Add(define.Key, define.Value);
                        }
                        string[] extensions = file.Extensions;
                        if (extensions.Length > 1)
                        {
                            properties.AddModule(extensions[extensions.Length - 2]);
                        }
                        bool isError;
                        using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            isError = !properties.Load(fs);
                        }
                        if (logger != null)
                        {
                            LogResult(properties, logger, isError);
                        }
                        if (!isError)
                        {
                            IFormatModule module; if (!properties.TryGetEmbedded(out module) || !(module is IPropertyProvider))
                            {
                                result = null;
                                isError = true;
                            }
                            else result = (module as IPropertyProvider);
                        }
                        else result = null;
                        return !isError;
                    }
                default:
                    {
                        IFormatModule formatter; if (FormatRegistry.TryGetModule(file.Extension, out formatter) && formatter is IPropertyProvider)
                        {
                            bool isError;
                            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                isError = !formatter.Load(fs, null);
                            }
                            if (logger != null)
                            {
                                LogResult(formatter, logger, isError);
                            }
                            if (!isError)
                            {
                                result = (formatter as IPropertyProvider);
                            }
                            else result = null;
                            return !isError;
                        }
                        else
                        {
                            result = null;
                            return false;
                        }
                    }
            }
        }
        /// <summary>
        /// Loads a property provider from this file's content if possible
        /// </summary>
        /// <param name="defines">A collection of defined constants to be processed by Alchemy</param>
        /// <param name="result">The resulting property provider if processing succeeded</param>
        /// <returns>True if the file has successfully been processed, false otherwise</returns>
        public static bool GetProperties(this FileDescriptor file, IEnumerable<KeyValuePair<string, string>> defines, out IPropertyProvider result)
        {
            return GetProperties(file, defines, null, out result);
        }
        /// <summary>
        /// Loads a property provider from this file's content if possible
        /// </summary>
        /// <param name="logger">A logging interface to write parser errors to</param>
        /// <param name="result">The resulting property provider if processing succeeded</param>
        /// <returns>True if the file has successfully been processed, false otherwise</returns>
        public static bool GetProperties(this FileDescriptor file, ILogSystem logger, out IPropertyProvider result)
        {
            return GetProperties(file, ArrayExtension.Empty<KeyValuePair<string, string>>(), logger, out result);
        }
        /// <summary>
        /// Loads a property provider from this file's content if possible
        /// </summary>
        /// <param name="result">The resulting property provider if processing succeeded</param>
        /// <returns>True if the file has successfully been processed, false otherwise</returns>
        public static bool GetProperties(this FileDescriptor file, out IPropertyProvider result)
        {
            return GetProperties(file, ArrayExtension.Empty<KeyValuePair<string, string>>(), null, out result);
        }

        private static void LogResult(PropertySheet properties, ILogSystem logger, bool isError)
        {
            if (isError)
            {
                foreach (string error in properties.Errors)
                    logger.Error(error);
            }
            else foreach (string error in properties.Errors)
                    logger.Warning(error);
        }
        private static void LogResult(IFormatModule formatter, ILogSystem logger, bool isError)
        {
            if (isError)
            {
                foreach (string error in formatter.Errors)
                    logger.Error(error);
            }
            else foreach (string error in formatter.Errors)
                    logger.Warning(error);
        }
    }
}
