// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// A file system data entry element
    /// </summary>
    [Serializable]
    public partial class FileDescriptor : FileSystemDescriptor
    {
        public override UInt32 Id
        {
            get { return GetAbsolutePath().Fnv32(); }
        }

        public override FileSystemEntryType Type
        {
            get { return FileSystemEntryType.File; }
        }

        protected PathDescriptor location;
        /// <summary>
        /// The grouping file system path entry of this data element
        /// </summary>
        public PathDescriptor Location
        {
            get { return location; }
        }

        protected string name;
        protected string extension;

        public override string Name
        {
            get { return name; }
        }
        public override string FullName
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(extension)) return Name;
                else return string.Concat(Name, ".", extension);
            }
        }

        /// <summary>
        /// The file extension this file system entry has
        /// </summary>
        public string Extension
        {
            get { return extension; }
        }
        /// <summary>
        /// The file extension this file system entry has
        /// </summary>
        public string[] Extensions
        {
            get 
            {
                string[] extensions = FullName.Split('.');
                Array.Copy(extensions, 1, extensions, 0, extensions.Length - 1);
                Array.Resize(ref extensions, extensions.Length - 1);

                return extensions;
            }
        }

        public override DateTime Timestamp
        {
            get { return new FileInfo(GetAbsolutePath()).LastWriteTime; }
        }

        public static bool operator ==(FileDescriptor left, FileDescriptor right)
        {
            return (left as object == right as object ||
                   (left as object != null && right as object != null &&
                    left.location == right.location && left.FullName == right.FullName));
        }
        public static bool operator !=(FileDescriptor left, FileDescriptor right)
        {
            return (left as object != right as object &&
                   (left as object == null || right as object == null ||
                    left.location != right.location || left.FullName != right.FullName));
        }

        /// <summary>
        /// Creates a new data element file system entry with the given name,
        /// grouped by a location description
        /// </summary>
        /// <param name="location">The location this element is grouped to</param>
        /// <param name="name">The full name of this element including extensions</param>
        public FileDescriptor(PathDescriptor location, string name)
        {
            this.location = location.Combine(Path.GetDirectoryName(name));
            this.name = Path.GetFileNameWithoutExtension(name);
            this.extension = Path.GetExtension(name).Trim('.');
        }
        /// <summary>
        /// Creates a new data element file system entry with the given name,
        /// grouped by a location description
        /// </summary>
        /// <param name="location">The location this element is grouped to</param>
        /// <param name="name">The full name of this element including extensions</param>
        public FileDescriptor(PathDescriptor location, string name, params object[] args)
            : this(location, string.Format(name, args))
        { }
        /// <summary>
        /// Creates a copy of an existing file system entry
        /// </summary>
        public FileDescriptor(FileDescriptor source)
        {
            this.location = source.location;
            this.extension = source.extension;
            this.name = source.name;
        }

        /// <summary>
        /// Creates a string that contains last count file extensions
        /// </summary>
        public string GetExtensions(int count)
        {
            if (count <= 0)
            {
                return string.Empty;
            }
            else
            {
                string[] extensions = FullName.Split('.');
                StringBuilder sb = new StringBuilder();
                for (int i = extensions.Length - count; i < extensions.Length; i++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('.');
                    }
                    sb.Append(extensions[i]);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates a new data element file system entry with modified extensions
        /// </summary>
        /// <returns></returns>
        public FileDescriptor ChangeExtensions(params string[] extensions)
        {
            StringBuilder sb = new StringBuilder();
            int idx = name.IndexOf('.');

            if (idx >= 0)
            {
                sb.Append(name.Substring(0, idx));
            }
            else sb.Append(name);
            for (int i = 0; i < extensions.Length; i++)
            {
                sb.Append('.');
                sb.Append(extensions[i]);
            }
            return new FileDescriptor(location, sb.ToString());
        }

        /// <summary>
        /// Appends the given name to this objects name
        /// </summary>
        /// <param name="name">A string to be appended to this objects name</param>
        /// <returns>The modified file system entry</returns>
        public FileDescriptor Append(string name)
        {
            FileDescriptor result = new FileDescriptor(this);
            result.name = string.Concat(this.name, name);

            return result;
        }

        /// <summary>
        /// Replaces this objects name with the given name
        /// </summary>
        /// <param name="name">A string to replace this objects name</param>
        /// <returns>The modified file system entry</returns>
        public FileDescriptor Replace(string name)
        {
            FileDescriptor result = new FileDescriptor(this);
            result.name = name;

            return result;
        }

        public override string GetAbsolutePath()
        {
            return location.GetAbsolutePath(FullName);
        }

        public override string GetRelativePath(FileSystemDescriptor root)
        {
            return Path.Combine(PathDescriptor.GetRelativePath(root, location.Uri), FullName);
        }

        public override void Create()
        {
            File.Create(GetAbsolutePath());
        }
        public override bool Exists()
        {
            return File.Exists(GetAbsolutePath());
        }

        public override void Equalize()
        {
            FileInfo fi = new FileInfo(GetAbsolutePath());
            location = new PathDescriptor(fi.DirectoryName);

            name = FileSystemDescriptor.GetExactPath(fi);
            extension = Path.GetExtension(name).Trim('.');
            name = Path.GetFileNameWithoutExtension(name);
        }

        /// <summary>
        /// Creates a stream object pointing to this element's physical storage
        /// </summary>
        /// <param name="mode">The desired creation mode</param>
        /// <param name="access">An access flag applied to the stream</param>
        /// <returns>The stream instance</returns>
        public FileStream Open(FileMode mode, FileAccess access)
        {
            return Open(mode, access, FileShare.None);
        }
        /// <summary>
        /// Creates a stream object pointing to this element's physical storage
        /// </summary>
        /// <param name="mode">The desired creation mode</param>
        /// <param name="access">An access flag applied to the stream</param>
        /// <param name="share">An access flag for parallel processing</param>
        /// <returns>The stream instance</returns>
        public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(GetAbsolutePath(), mode, access, share);
        }

        public override void Delete()
        {
            FileInfo fi = new FileInfo(GetAbsolutePath());
            fi.Attributes = FileAttributes.Normal;
            fi.Delete();
        }

        public override int GetHashCode()
        {
            return GetAbsolutePath().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (this == obj as FileDescriptor);
        }

        public override string ToString()
        {
            return GetAbsolutePath();
        }
    }
}
