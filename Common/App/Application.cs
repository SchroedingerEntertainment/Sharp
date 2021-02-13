// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace System.Runtime
{
    /// <summary>
    /// Provides static methods and properties to handle tool applications with
    /// auto configuring from JSON manifest and program arguments
    /// </summary>
    public static partial class Application
    {
        public const string CacheDirectoryName = ".cache";
        public const string ConfigDirectoryName = "Config";

        private static FileStream assemblyLock;
        private static atomic_bool hasErrors;

        /// <summary>
        /// Returns this Application's file system name
        /// </summary>
        public static string Name
        {
            get { return self.Name; }
        }

        private static FileDescriptor self;
        /// <summary>
        /// Returns the full qualified file system location this assembly is
        /// based on
        /// </summary>
        public static FileDescriptor Self
        {
            get { return self; }
        }

        private static FileDescriptor parent;
        /// <summary>
        /// Returns the full qualified file system location of the module
        /// this Process was started by
        /// </summary>
        public static FileDescriptor Parent
        {
            get { return parent; }
        }

        private static PathDescriptor rootPath;
        /// <summary>
        /// The absolute path to the location this Application is 
        /// grouped by
        /// </summary>
        public static PathDescriptor RootPath
        {
            get { return rootPath; }
        }

        private static PathDescriptor workerPath;
        /// <summary>
        /// The absolute path to the location this Application should
        /// work at
        /// </summary>
        public static PathDescriptor WorkerPath
        {
            get { return workerPath; }
        }

        private static PathDescriptor sdkRoot;
        /// <summary>
        /// The absolute path to the SDK location
        /// </summary>
        public static PathDescriptor SdkRoot
        {
            get
            {
                if (sdkRoot == null)
                    lock (rootPath)
                    {
                        if (sdkRoot == null)
                            sdkRoot = new PathDescriptor(GetTopLevelPath(RootPath.GetAbsolutePath()));
                    }
                return sdkRoot;
            }
        }

        private static PathDescriptor sdkCache;
        /// <summary>
        /// The absolute path to the cache location in the SDK
        /// </summary>
        public static PathDescriptor SdkCache
        {
            get
            {
                if (sdkCache == null)
                    lock (rootPath)
                    {
                        if (sdkCache == null)
                        {
                            sdkCache = SdkRoot.Combine(CacheDirectoryName);
                        }
                    }
                return sdkCache;
            }
        }

        private static PathDescriptor sdkConfig;
        /// <summary>
        /// The absolute path to the global config location assigned 
        /// to this assembly in the SDK
        /// </summary>
        public static PathDescriptor SdkConfig
        {
            get
            {
                if (sdkConfig == null)
                    lock (rootPath)
                    {
                        if (sdkConfig == null)
                            sdkConfig = SdkRoot.Combine(ConfigDirectoryName);
                    }
                return sdkConfig;
            }
        }

        private static PathDescriptor sdkLocalConfig;
        /// <summary>
        /// The absolute path to the global config location assigned 
        /// to this assembly in the SDK
        /// </summary>
        public static PathDescriptor SdkLocalConfig
        {
            get
            {
                if (sdkLocalConfig == null)
                    lock (rootPath)
                    {
                        if (sdkLocalConfig == null)
                            sdkLocalConfig = SdkConfig.Combine(Name);
                    }
                return sdkLocalConfig;
            }
        }

        private static PathDescriptor projectRoot;
        /// <summary>
        /// The absolute path to the root worker path
        /// </summary>
        public static PathDescriptor ProjectRoot
        {
            get
            {
                if (projectRoot == null)
                    lock (rootPath)
                    {
                        if (projectRoot == null)
                            projectRoot = new PathDescriptor(GetTopLevelPath());
                    }
                return projectRoot;
            }
        }

        private static PathDescriptor cacheDirectory;
        /// <summary>
        /// The absolute path to the cache location in current project
        /// </summary>
        public static PathDescriptor CacheDirectory
        {
            get
            {
                if (cacheDirectory == null)
                    lock (rootPath)
                    {
                        if (cacheDirectory == null)
                        {
                            cacheDirectory = ProjectRoot.Combine(CacheDirectoryName);
                            cacheDirectory = cacheDirectory.Combine(Name);
                        }
                    }
                return cacheDirectory;
            }
        }

        private static PathDescriptor configDirectory;
        /// <summary>
        /// The absolute path to the global config location assigned 
        /// to this assembly in current worker path
        /// </summary>
        public static PathDescriptor ConfigDirectory
        {
            get
            {
                if (configDirectory == null)
                    lock (rootPath)
                    {
                        if (configDirectory == null)
                            configDirectory = ProjectRoot.Combine(ConfigDirectoryName);
                    }
                return configDirectory;
            }
        }

        private static PathDescriptor localConfigDirectory;
        /// <summary>
        /// The absolute path to the global config location assigned 
        /// to this assembly in current worker path
        /// </summary>
        public static PathDescriptor LocalConfigDirectory
        {
            get
            {
                if (localConfigDirectory == null)
                    lock (rootPath)
                    {
                        if (localConfigDirectory == null)
                            localConfigDirectory = ConfigDirectory.Combine(Name);
                    }
                return localConfigDirectory;
            }
        }

        private static Version version;
        /// <summary>
        /// Obtains the compiler related file version
        /// </summary>
        public static Version Version
        {
            get 
            {
                if (version == null)
                    lock (rootPath)
                    {
                        if (version == null)
                            version = new AssemblyName(Assembly.GetEntryAssembly().FullName).Version;
                    }
                return version;
            }
        }

        /// <summary>
        /// Gets last modification timestamp of this Application
        /// </summary>
        public static DateTime Timestamp
        {
            get { return self.Timestamp; }
        }

        /// <summary>
        /// Time since the application started
        /// </summary>
        public static DateTime StartupTime
        {
            get { return Process.GetCurrentProcess().StartTime; }
        }

        private static bool isAlreadyRunning;
        /// <summary>
        /// Determines if the application has already been started in another process
        /// </summary>
        public static bool IsAlreadyRunning
        {
            get
            {
                if (assemblyLock == null)
                {
                    assemblyLock = self.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                    try
                    {
                        isAlreadyRunning = false;
                    }
                    catch (IOException)
                    {
                        isAlreadyRunning = true;
                    }
                }
                return isAlreadyRunning;
            }
        }

        static Application()
        {
            string parentProcess = Environment.GetEnvironmentVariable(ProcessExtension.ProcessChildIdentifier);
            if (!string.IsNullOrWhiteSpace(parentProcess) && File.Exists(parentProcess))
            {
                parent = FileDescriptor.Create(parentProcess);
            }

            self = FileDescriptor.Create(new Uri(Process.GetCurrentProcess().MainModule.FileName).LocalPath);
            rootPath = self.Location;

            workerPath = new PathDescriptor(Path.GetFullPath(Environment.CurrentDirectory));
            targetVersion = VersionFlags.Undefined;

            logLock = new Threading.Spinlockʾ();
            logSystem = ConsoleLog.Instance;
        }

        /// <summary>
        /// Detects the logical top level location grouping this Application and a subset
        /// of necessary directories
        /// </summary>
        /// <param name="basePath">The basic location to start searching</param>
        /// <returns>The logical absolute top level location</returns>
        public static string GetTopLevelPath(string basePath = null)
        {
            if (string.IsNullOrWhiteSpace(basePath))
                basePath = workerPath.GetAbsolutePath();

            string tmp = null;
            if (!string.IsNullOrWhiteSpace(tmp = LinearFindDirectory(basePath, ConfigDirectoryName)))
            {
                return Path.GetDirectoryName(tmp);
            }
            else if (!string.IsNullOrWhiteSpace(tmp = LinearFindDirectory(rootPath.GetAbsolutePath(), ConfigDirectoryName)))
            {
                return Path.GetDirectoryName(tmp);
            }
            else return rootPath.GetAbsolutePath();
        }
        private static string LinearFindDirectory(string path, string directory)
        {
            string current = Path.Combine(path, directory);
            if (Directory.Exists(current))
                return current;

            path = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return LinearFindDirectory(path, directory);
        }
    }
}
