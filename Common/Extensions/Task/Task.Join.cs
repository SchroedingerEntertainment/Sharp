// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static partial class TaskExtension
    {
        /// <summary>
        /// Joins the task until execution ends
        /// </summary>
        /// <returns>True if the task was completed successfully, false otherwise</returns>
        public static bool Join(this Task task)
        {
            try
            {
                try
                {
                    task.RunSynchronously();
                }
                catch (InvalidOperationException)
                {
                    task.GetAwaiter().GetResult();
                }
            }
            catch (TaskCanceledException)
            { }
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: return true;
                default: return false;
            }
        }
    }
}