// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SE.Parsing;

namespace SE.Alchemy
{
    /// <summary>
    /// Transforms text data from the Alchemy preprocessor into the defined format and
    /// provides an interface to access data properties of the format if possible
    /// </summary>
    public partial class PropertySheet : IParserContext
    {
        public const string FilterPrefix = "\"?";

        FileDescriptor target;
        Encoding encoding;

        IPropertyProvider propertyModule;
        Dictionary<string, IFormatModule> modules;

        Dictionary<string, string> defines;
        /// <summary>
        /// A collection of macros defined when parsing text
        /// </summary>
        public Dictionary<string, string> Defines
        {
            get { return defines; }
        }

        /// <summary>
        /// Determines if a format module was embedded
        /// </summary>
        public bool HasFormat
        {
            get { return propertyModule != null; }
        }

        List<string> errors;
        /// <summary>
        /// A collection of error messages since last parse
        /// </summary>
        public IEnumerable<string> Errors
        {
            get
            {
                if (propertyModule != null)
                {
                    return errors.Union((propertyModule as IFormatModule).Errors);
                }
                else return errors;
            }
        }

        /// <summary>
        /// Determines if has parser errors
        /// </summary>
        public bool HasErrors
        {
            get
            {
                bool hasErrors = (errors.Count > 0);
                if (propertyModule != null)
                {
                    return (propertyModule as IFormatModule).HasErrors | hasErrors;
                }
                else return hasErrors;
            }
        }

        /// <summary>
        /// Creates a new instance with a provided format module ID
        /// </summary>
        /// <param name="path">The location of where imports should be based on</param>
        /// <param name="format">A known format module ID to embedd</param>
        public PropertySheet(FileDescriptor path, string format)
        {
            this.target = path;
            this.modules = new Dictionary<string, IFormatModule>();
            this.defines = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(format))
            {
                AddModule(format);
            }
            this.errors = new List<string>();
        }
        /// <summary>
        /// Creates a new instance with a provided format module ID
        /// </summary>
        /// <param name="path">The location of where imports should be based on</param>
        /// <param name="format">A known format module ID to embedd</param>
        public PropertySheet(string path, string format)
            : this(FileDescriptor.Create(path), format)
        { }
        /// <summary>
        /// Creates a new instance without a format module
        /// </summary>
        /// <param name="location">The location of where imports should be based on</param>
        public PropertySheet(FileDescriptor path)
            : this(path, null)
        { }
        /// <summary>
        /// Creates a new instance without a format module
        /// </summary>
        /// <param name="path">The location of where imports should be based on</param>
        public PropertySheet(string path)
            : this(path, null)
        { }

        public bool AddModule(string id)
        {
            id = id.ToLowerInvariant();
            if (modules.ContainsKey(id))
            {
                return true;
            }
            IFormatModule module; if (FormatRegistry.TryGetModule(id, out module))
            {
                modules.Add(id, module);
                if (!HasFormat)
                {
                    propertyModule = (module as IPropertyProvider);
                }
                return true;
            }
            else return false;
        }

        public string Transform(Token token, string input)
        {
            foreach (IFormatModule module in modules.Values)
            {
                input = module.Transform(token, input);
            }
            return input;
        }

        public virtual bool Load(Stream stream, Encoding encoding)
        {
            propertyModule = null;
            modules.Clear();
            errors.Clear();

            using (ProcessorStream processor = new ProcessorStream(this, stream, target))
            {
                foreach (KeyValuePair<string, string> define in defines)
                {
                    processor.Define(define.Key, define.Value);
                }
                this.encoding = processor.Encoding;
                while (!processor.EndOfStream && !HasFormat)
                {
                    processor.Fill();
                }
                errors.AddRange(processor.Errors);
                if (HasFormat)
                {
                    return (propertyModule as IFormatModule).Load(processor, processor.Encoding);
                }
                else return !HasErrors;
            }
        }
        /// <summary>
        /// Try to load the provided stream into the given data format module
        /// </summary>
        /// <param name="stream">The input text stream to load</param>
        /// <returns>True if loaded successfully, false otherwise</returns>
        public virtual bool Load(Stream stream)
        {
            return Load(stream, null);
        }
        
        public bool ResolveFileReference(object context, ref string path, ref string prefix, out Stream stream)
        {
            try
            {
                if (path.StartsWith(FilterPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    stream = new MemoryStream();
                    List<FileSystemDescriptor> results = CollectionPool<List<FileSystemDescriptor>, FileSystemDescriptor>.Get();
                    MemoryStream ms = MemoryPool<MemoryStream>.Get();
                    try
                    {
                        ms.Write(encoding.GetBytes(prefix));
                        if ((context as FileDescriptor).Location.FindFiles(path.Substring(FilterPrefix.Length).TrimEnd('\"'), results) > 0)
                            foreach (FileDescriptor resource in results)
                                using (FileStream fs = resource.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                                using (StreamReader sr = new StreamReader(fs, true))
                                {
                                    ms.Position = 0;
                                    ms.CopyTo(stream);

                                    stream.Write(encoding.GetBytes(sr.ReadToEnd()));
                                }
                    }
                    finally
                    {
                        CollectionPool<List<FileSystemDescriptor>, FileSystemDescriptor>.Return(results);
                        MemoryPool<MemoryStream>.Return(ms);
                    }
                    stream.Position = 0;
                    prefix = null;
                    return true;
                }
                else
                {
                    path = Environment.ExpandEnvironmentVariables(path.Substring(1).TrimEnd('\"'));
                    if (!File.Exists(path))
                    {
                        path = target.Location.Combine(path).GetAbsolutePath();
                    }
                    if (File.Exists(path))
                    {
                        stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                        return true;
                    }
                    else stream = null;
                }
            }
            catch (Exception er)
            {
                errors.Add(er.Message);
                stream = null;
            }
            return false;
        }

        /// <summary>
        /// Tries to obtain the embedded format handler used to process the data
        /// </summary>
        /// <param name="module">The embedded format module</param>
        /// <returns>True if a module is embedded into this instance, false otherwise</returns>
        public bool TryGetEmbedded(out IFormatModule module)
        {
            module = (propertyModule as IFormatModule);
            return HasFormat;
        }
    }
}
