﻿// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    public partial class PathDescriptor
    {
        /// <summary>
        /// Does a file system lookup and returns any entry of type File that matches the
        /// provided pattern
        /// </summary>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public List<FileSystemDescriptor> FindFiles(string pattern, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            return FindEntries(this, pattern, PathEntryOption.File, direction);
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type File that matches the
        /// provided pattern
        /// </summary>
        /// <param name="filter">A pattern that will be translated into a filter object</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public List<FileSystemDescriptor> FindFiles(Filter filter, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            return FindEntries(this, filter, PathEntryOption.File, direction);
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type File that matches the
        /// provided filter
        /// </summary>
        /// <param name="filter">A filter object to apply to the</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public int FindFiles(Filter filter, ICollection<FileSystemDescriptor> files, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            FindEntries(this, filter, PathEntryOption.File, direction, files);
            return files.Count;
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type File that matches the
        /// provided filter
        /// </summary>
        /// <param name="pattern">A filter object to apply to the</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public int FindFiles(string pattern, ICollection<FileSystemDescriptor> files, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            FindEntries(this, pattern, PathEntryOption.File, direction, files);
            return files.Count;
        }

        /// <summary>
        /// Does a file system lookup and returns any entry of type File that matches the
        /// provided pattern
        /// </summary>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="file">The resulting file system entry</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>True if at least one file system entry matched the pattern, false otherwise</returns>
        public bool FindFile(string pattern, out FileDescriptor file, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            List<FileSystemDescriptor> files = FindFiles(pattern, direction);
            if (files.Count != 0) file = (files[0] as FileDescriptor);
            else file = null;

            return (file != null);
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type File that matches the
        /// provided filter
        /// </summary>
        /// <param name="pattern">A filter object to apply to the</param>
        /// /// <param name="file">The resulting file system entry</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>True if at least one file system entry matched the pattern, false otherwise</returns>
        public bool FindFile(Filter filter, out FileDescriptor file, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            List<FileSystemDescriptor> files = FindFiles(filter, direction);
            if (files.Count != 0) file = (files[0] as FileDescriptor);
            else file = null;

            return (file != null);
        }

        private static void FindFiles(Filter filter, DirectoryInfo directory, string relativePath, bool reverseLookup, bool iterate, ICollection<FileSystemDescriptor> items)
        {
            try
            {
                foreach (FileInfo file in directory.EnumerateFiles())
                {
                    string path = PathDescriptor.Normalize(Path.Combine(relativePath, file.Name));
                    if (filter.IsMatch(path.Split('/')) && !file.Attributes.HasFlag(FileAttributes.ReparsePoint))
                        items.Add(new FileDescriptor(new PathDescriptor(file.DirectoryName), file.Name));
                }
                if (iterate)
                {
                    foreach (DirectoryInfo dir in directory.EnumerateDirectories())
                    {
                        string path = relativePath + dir.Name;
                        FindFiles(filter, dir, path + "/", false, true, items);
                    }
                }
                if (iterate && reverseLookup && items.Count == 0) FindFiles(filter, directory.Parent, "", reverseLookup, true, items);
            }
            catch { }
        }
    }
}
