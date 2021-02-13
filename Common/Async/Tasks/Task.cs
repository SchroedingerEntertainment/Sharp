// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static partial class Taskʾ
    {
        /// <summary>
        /// Returns the ID of the currently executing Task.
        /// </summary>
        public static int? CurrentId
        {
            get { return Task.CurrentId; }
        }

        /// <summary>
        /// Provides access to factory methods for creating and configuring Task and 
        /// Task<TResult> instances.
        /// </summary>
        public static TaskFactory Factory
        {
            get { return Task.Factory; }
        }

        static Taskʾ()
        { 
            #if net40
            canceledTask = new TaskCompletionSource<bool>();
            canceledTask.TrySetCanceled();
            #endif

            completedTask = new TaskCompletionSource<bool>();
            completedTask.SetResult(false);
        }

        /// <summary>
        /// Waits for all of the provided Task objects to complete execution within a specified 
        /// number of milliseconds or until the wait is cancelled.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the tasks to complete.</param>
        /// <returns>true if all of the Task instances completed execution within the allotted time; otherwise, false.</returns>
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return Task.WaitAll(tasks, millisecondsTimeout, cancellationToken);
        }
        /// <summary>
        /// Waits for all of the provided Task objects to complete execution unless the wait is cancelled.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the tasks to complete.</param>
        public static void WaitAll(Task[] tasks, CancellationToken cancellationToken)
        {
            Task.WaitAll(tasks, cancellationToken);
        }
        /// <summary>
        /// Waits for all of the provided Task objects to complete execution within a specified number of milliseconds.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.</param>
        /// <returns>true if all of the Task instances completed execution within the allotted time; otherwise, false.</returns>
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout)
        {
            return Task.WaitAll(tasks, millisecondsTimeout);
        }
        /// <summary>
        /// Waits for all of the provided cancellable Task objects to complete execution within a specified time interval.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="timeout">
        /// A TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds
        /// to wait indefinitely.
        /// </param>
        /// <returns>true if all of the Task instances completed execution within the allotted time; otherwise, false.</returns>
        public static bool WaitAll(Task[] tasks, TimeSpan timeout)
        {
            return Task.WaitAll(tasks, timeout);
        }
        /// <summary>
        /// Waits for all of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        public static void WaitAll(params Task[] tasks)
        {
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Waits for any of the provided Task objects to complete execution within a specified number of
        /// milliseconds or until a cancellation token is cancelled.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>The index of the completed task in the tasks array argument, or -1 if the timeout occurred.</returns>
        public static int WaitAny(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return Task.WaitAny(tasks, millisecondsTimeout, cancellationToken);
        }
        /// <summary>
        /// Waits for any of the provided Task objects to complete execution within a specified number of milliseconds.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.</param>
        /// <returns>The index of the completed task in the tasks array argument, or -1 if the timeout occurred.</returns>
        public static int WaitAny(Task[] tasks, int millisecondsTimeout)
        {
            return Task.WaitAny(tasks, millisecondsTimeout);
        }
        /// <summary>
        /// Waits for any of the provided Task objects to complete execution unless the wait is cancelled.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for a task to complete.</param>
        /// <returns>The index of the completed task in the tasks array argument.</returns>
        public static int WaitAny(Task[] tasks, CancellationToken cancellationToken)
        {
            return Task.WaitAny(tasks, cancellationToken);
        }
        /// <summary>
        /// Waits for any of the provided Task objects to complete execution within a specified time interval.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <param name="timeout">
        /// A TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1
        /// milliseconds to wait indefinitely.
        /// </param>
        /// <returns>The index of the completed task in the tasks array argument, or -1 if the timeout occurred.</returns>
        public static int WaitAny(Task[] tasks, TimeSpan timeout)
        {
            return Task.WaitAny(tasks, timeout);
        }
        /// <summary>
        /// Waits for any of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="tasks">An array of Task instances on which to wait.</param>
        /// <returns>The index of the completed Task object in the tasks array.</returns>
        public static int WaitAny(params Task[] tasks)
        {
            return Task.WaitAny(tasks);
        }
    }
}
