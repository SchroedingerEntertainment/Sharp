// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.App
{
    /// <summary>
    /// Defines the main entry point of the wrapped App
    /// </summary>
    /// <remarks>
    /// This is a partial class that expects a static function SE.App.Activity GetStartupActivity(System.string[] args)
    /// to deliver the activity to run when entering the main procedure
    /// </remarks>
    public partial class Program
    {
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                args = new string[] { "-?" };
            }
            Activity activity = GetStartupActivity(args);
            if (activity != null)
            {
                return activity.Run(args);
            }
            else return int.MinValue;
        }
    }
}