// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if net40 || net403 || net45 || net451 || net452 || net46 || net461 || net462 || net47 || net471 || net472
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Policy;

namespace System
{
    /// <summary>
    /// Represents an application domain, which is an isolated environment where applications 
    /// execute. This class cannot be inherited
    /// </summary>
    public static class AppDomainʾ
    {
        [Guid("CB2F6722-AB3A-11D2-9C40-00C04FA30A3E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ICorRuntimeHost
        {
            void _VtblGap_10();
            void GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)] out object appDomain);
            void EnumDomains(out IntPtr enumHandle);
            void NextDomain(IntPtr enumHandle, [MarshalAs(UnmanagedType.IUnknown)] ref object appDomain);
            void CloseEnum(IntPtr enumHandle);
        }
        /// <summary>
        /// A cross-domain aware base object
        /// </summary>
        public class ReferenceObject : MarshalByRefObject, IDisposable
        {
            bool disposed;
            /// <summary>
            /// Get if the object has been disposed
            /// </summary>
            public bool IsDisposed
            {
                get { return disposed; }
            }

            /// <summary>
            /// Creates a new cross-domain aware object instance
            /// </summary>
            public ReferenceObject()
            {}
            ~ReferenceObject()
            {
                Dispose(false);
            }
            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    RemotingServices.Disconnect(this);
                    disposed = true;
                }
            }
        }

        private static readonly ICorRuntimeHost host;
        /// <summary>
        /// Gets the root application domain for the current Process
        /// </summary>
        public static AppDomain DefaultDomain
        {
            get 
            {
                object result;
                host.GetDefaultDomain(out result);
                return (result as AppDomain);
            }
        }

        /// <summary>
        /// Gets the current application domain for the current Thread
        /// </summary>
        public static AppDomain CurrentDomain
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>
        /// Gets the number of bytes that survived the last collection and that are known 
        /// to be referenced by the current application domain
        /// </summary>
        public static long MonitoringSurvivedProcessMemorySize
        {
            get { return AppDomain.MonitoringSurvivedProcessMemorySize; }
        }
        /// <summary>
        /// Gets or sets a value that indicates whether CPU and memory monitoring of application 
        /// domains is enabled for the current process. Once monitoring is enabled for a process, 
        /// it cannot be disabled
        /// </summary>
        public static bool MonitoringIsEnabled
        {
            get { return AppDomain.MonitoringIsEnabled; }
            set { AppDomain.MonitoringIsEnabled = value; }
        }

        static AppDomainʾ()
        {
            host = (ICorRuntimeHost)Activator.CreateInstance(Type.GetTypeFromCLSID(Guid.Parse("CB2F6723-AB3A-11D2-9C40-00C04FA30A3E")));
        }

        /// <summary>
        /// Creates a new application domain with the given name, using evidence, application base path,
        /// relative search path, and a parameter that specifies whether a shadow copy of an assembly is
        /// to be loaded into the application domain. Specifies a callback method that is invoked when the
        /// application domain is initialized, and an array of string arguments to pass the callback method
        /// </summary>
        /// <param name="friendlyName">
        /// The friendly name of the domain. This friendly name can be displayed in user interfaces to identify 
        /// the domain. For more information, see FriendlyName
        /// </param>
        /// <param name="securityInfo">
        /// Evidence that establishes the identity of the code that runs in the application domain. Pass null 
        /// to use the evidence of the current application domain
        /// </param>
        /// <param name="appBasePath">
        /// The base directory that the assembly resolver uses to probe for assemblies. For more information,
        /// see BaseDirectory
        /// </param>
        /// <param name="appRelativeSearchPath">
        /// The path relative to the base directory where the assembly resolver should probe for private assemblies.
        /// For more information, see RelativeSearchPath
        /// </param>
        /// <param name="shadowCopyFiles">true to load a shadow copy of an assembly into the application domain</param>
        /// <param name="adInit">
        /// An AppDomainInitializer delegate that represents a callback method to invoke when the new AppDomain 
        /// object is initialized
        /// </param>
        /// <param name="adInitArgs">
        /// An array of string arguments to be passed to the callback represented by adInit, when the new AppDomain 
        /// object is initialized
        /// </param>
        /// <remarks>
        /// The method represented by adInit is executed in the context of the newly created application domain.
        /// If securityInfo is not supplied, the evidence from the current application domain is used.
        /// For more information about shadow copying, see ShadowCopyFiles and Shadow Copying Assemblies
        /// </remarks>
        /// <returns>The newly created application domain</returns>
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles, AppDomainInitializer adInit, string[] adInitArgs)
        {
            return AppDomain.CreateDomain(friendlyName, securityInfo, appBasePath, appRelativeSearchPath, shadowCopyFiles, adInit, adInitArgs);
        }
        /// <summary>
        /// Creates a new application domain using the specified name, evidence, application domain setup
        /// information, default permission set, and array of fully trusted assemblies
        /// </summary>
        /// <param name="friendlyName">
        /// The friendly name of the domain. This friendly name can be displayed in user interfaces to identify 
        /// the domain. For more information, see the description of FriendlyName
        /// </param>
        /// <param name="securityInfo">
        /// Evidence that establishes the identity of the code that runs in the application domain. Pass null
        /// to use the evidence of the current application domain
        /// </param>
        /// <param name="info">An object that contains application domain initialization information</param>
        /// <param name="grantSet">
        /// A default permission set that is granted to all assemblies loaded into the new application 
        /// domain that do not have specific grants
        /// </param>
        /// <param name="fullTrustAssemblies">
        /// An array of strong names representing assemblies to be considered fully trusted in the new
        /// application domain.
        /// </param>
        /// <remarks>
        /// You must set the ApplicationBase property of the AppDomainSetup object that you supply for info. Otherwise, an exception is thrown.
        /// If securityInfo is not supplied, the evidence from the current application domain is used.
        /// The information provided for grantSet and fullTrustAssemblies is used to create an ApplicationTrust object for the new application domain
        /// </remarks>
        /// <returns>The newly created application domain</returns>
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info, PermissionSet grantSet, params StrongName[] fullTrustAssemblies)
        {
            return AppDomain.CreateDomain(friendlyName, securityInfo, info, grantSet, fullTrustAssemblies);
        }
        /// <summary>
        /// Creates a new application domain with the given name, using evidence, application base path, relative
        /// search path, and a parameter that specifies whether a shadow copy of an assembly is to be loaded into
        /// the application domain
        /// </summary>
        /// <param name="friendlyName">
        /// The friendly name of the domain. This friendly name can be displayed in user interfaces to identify 
        /// the domain. For more information, see FriendlyName
        /// </param>
        /// <param name="securityInfo">
        /// Evidence that establishes the identity of the code that runs in the application domain. Pass null
        /// to use the evidence of the current application domain
        /// </param>
        /// <param name="appBasePath">
        /// The base directory that the assembly resolver uses to probe for assemblies. For more information,
        /// see BaseDirectory
        /// </param>
        /// <param name="appRelativeSearchPath">
        /// The path relative to the base directory where the assembly resolver should probe for private assemblies.
        /// For more information, see RelativeSearchPath
        /// </param>
        /// <param name="shadowCopyFiles">If true, a shadow copy of an assembly is loaded into this application domain</param>
        /// <remarks>
        /// If securityInfo is not supplied, the evidence from the current application domain is used.
        /// For more information about shadow copying, see ShadowCopyFiles and Shadow Copying Assemblies
        /// </remarks>
        /// <returns>The newly created application domain</returns>
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
        {
            return AppDomain.CreateDomain(friendlyName, securityInfo, appBasePath, appRelativeSearchPath, shadowCopyFiles);
        }
        /// <summary>
        /// Creates a new application domain using the specified name, evidence, and application domain setup information
        /// </summary>
        /// <param name="friendlyName">
        /// The friendly name of the domain. This friendly name can be displayed in user interfaces to identify the domain. 
        /// For more information, see FriendlyName
        /// </param>
        /// <param name="securityInfo">
        /// Evidence that establishes the identity of the code that runs in the application domain. Pass null to use 
        /// the evidence of the current application domain
        /// </param>
        /// <param name="info">An object that contains application domain initialization information</param>
        /// <remarks>
        /// If info is not supplied, this method overload uses the AppDomainSetup information from the default application domain.
        /// If securityInfo is not supplied, the evidence from the current application domain is used
        /// </remarks>
        /// <returns>The newly created application domain</returns>
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info)
        {
            return AppDomain.CreateDomain(friendlyName, securityInfo, info);
        }
        /// <summary>
        /// Creates a new application domain with the given name using the supplied evidence
        /// </summary>
        /// <param name="friendlyName">
        /// The friendly name of the domain. This friendly name can be displayed in user interfaces to 
        /// identify the domain. For more information, see FriendlyName
        /// </param>
        /// <param name="securityInfo">
        /// Evidence that establishes the identity of the code that runs in the application domain. Pass 
        /// null to use the evidence of the current application domain
        /// </param>
        /// <remarks>
        /// This method overload uses the AppDomainSetup information from the default application domain.
        /// If securityInfo is not supplied, the evidence from the current application domain is used
        /// </remarks>
        /// <returns>The newly created application domain</returns>
        public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo)
        {
            return AppDomain.CreateDomain(friendlyName, securityInfo);
        }
        /// <summary>
        /// Creates a new application domain with the specified name
        /// </summary>
        /// <param name="friendlyName">The friendly name of the domain</param>
        /// <remarks>
        /// The friendlyName parameter is intended to identify the domain in a manner that is meaningful 
        /// to humans. This string should be suitable for display in user interfaces.
        /// This method overload uses the AppDomainSetup information from the default application domain
        /// </remarks>
        /// <returns>The newly created application domain</returns>
        public static AppDomain CreateDomain(string friendlyName)
        {
            return AppDomain.CreateDomain(friendlyName);
        }

        /// <summary>
        /// Unloads the specified application domain
        /// </summary>
        /// <remarks>
        /// In the .NET Framework version 2.0 there is a thread dedicated to unloading application domains. 
        /// This improves reliability, especially when the .NET Framework is hosted. When a thread calls Unload, 
        /// the target domain is marked for unloading. The dedicated thread attempts to unload the domain, and 
        /// all threads in the domain are aborted. If a thread does not abort, for example because it is 
        /// executing unmanaged code, or because it is executing a finally block, then after a period of 
        /// time a CannotUnloadAppDomainException is thrown in the thread that originally called Unload. If 
        /// the thread that could not be aborted eventually ends, the target domain is not unloaded. Thus, 
        /// in the .NET Framework version 2.0 domain is not guaranteed to unload, because it might not be 
        /// possible to terminate executing threads.
        /// 
        /// Note
        /// In some cases, calling Unload causes an immediate CannotUnloadAppDomainException, for example if it 
        /// is called in a finalizer.
        /// 
        /// The threads in domain are terminated using the Abort method, which throws a ThreadAbortException in the 
        /// thread.Although the thread should terminate promptly, it can continue executing for an unpredictable 
        /// amount of time in a finally clause
        /// </remarks>
        /// <param name="domain">An application domain to unload</param>
        public static void Unload(AppDomain domain)
        {
            AppDomain.Unload(domain);
        }

        /// <summary>
        /// Enumerates all currently-loaded AppDomains.
        /// </summary>
        public static IEnumerable<AppDomain> GetDomains()
        {
            IntPtr enumeration; host.EnumDomains(out enumeration);
            try
            {
                while (true)
                {
                    object domain = null;
                    host.NextDomain(enumeration, ref domain);
                    if (domain == null)
                    {
                        yield break;
                    }
                    yield return (AppDomain)domain;
                }
            }
            finally
            {
                host.CloseEnum(enumeration);
            }
        }
    }
}
#endif