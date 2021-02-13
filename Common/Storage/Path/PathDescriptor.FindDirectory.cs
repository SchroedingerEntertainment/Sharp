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
        /// Does a file system lookup and returns any entry of type Directory that matches the
        /// provided pattern
        /// </summary>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public List<FileSystemDescriptor> FindDirectories(string pattern, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            return FindEntries(this, pattern, PathEntryOption.Directory, direction);
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type Directory that matches the
        /// provided filter
        /// </summary>
        /// <param name="pattern">A filter object to apply to the</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public List<FileSystemDescriptor> FindDirectories(Filter filter, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            return FindEntries(this, filter, PathEntryOption.Directory, direction);
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type Directory that matches the
        /// provided pattern
        /// </summary>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public int FindDirectories(string pattern, ICollection<FileSystemDescriptor> directories, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            FindEntries(this, pattern, PathEntryOption.Directory, direction, directories);
            return directories.Count;
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type Directory that matches the
        /// provided filter
        /// </summary>
        /// <param name="pattern">A filter object to apply to the</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>The resulting list of file system entries</returns>
        public int FindDirectories(Filter filter, ICollection<FileSystemDescriptor> directories, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            FindEntries(this, filter, PathEntryOption.Directory, direction, directories);
            return directories.Count;
        }

        /// <summary>
        /// Does a file system lookup and returns any entry of type Directory that matches the
        /// provided pattern
        /// </summary>
        /// <param name="pattern">A pattern that will be translated into a filter object</param>
        /// <param name="location">The resulting file system entry</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>True if at least one file system entry matched the pattern, false otherwise</returns>
        public bool FindDirectory(string pattern, out PathDescriptor location, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            List<FileSystemDescriptor> files = FindDirectories(pattern, direction);
            if (files.Count != 0) location = (files[0] as PathDescriptor);
            else location = null;

            return (location != null);
        }
        /// <summary>
        /// Does a file system lookup and returns any entry of type Directory that matches the
        /// provided filter
        /// </summary>
        /// <param name="pattern">A filter object to apply to the</param>
        /// <param name="location">The resulting file system entry</param>
        /// <param name="direction">The direction to traverse the file system tree</param>
        /// <returns>True if at least one file system entry matched the pattern, false otherwise</returns>
        public bool FindDirectory(Filter filter, out PathDescriptor location, PathSeekOptions direction = PathSeekOptions.Forward)
        {
            List<FileSystemDescriptor> files = FindDirectories(filter, direction);
            if (files.Count != 0) location = (files[0] as PathDescriptor);
            else location = null;

            return (location != null);
        }

        private static void FindDirectories(Filter filter, DirectoryInfo directory, string relativePath, bool reverseLookup, bool iterate, ICollection<FileSystemDescriptor> items)
        {
            try
            {
                foreach (DirectoryInfo dir in directory.EnumerateDirectories())
                {
                    string path = PathDescriptor.Normalize(Path.Combine(relativePath, dir.Name));
                    if (filter.IsMatch(path.Split('/')))
                        items.Add(new PathDescriptor(dir.FullName));

                    if (iterate)
                    {
                        path = relativePath + dir.Name;
                        FindDirectories(filter, dir, path + "/", false, true, items);
                    }
                }
                if (iterate && reverseLookup && items.Count == 0) FindDirectories(filter, directory.Parent, "", reverseLookup, true, items);
            }
            catch { }
        }
    }
}
