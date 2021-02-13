// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static partial class Taskʾ
    {
        #if net40
        private readonly static TaskCompletionSource<bool> canceledTask;
        #endif
        private readonly static TaskCompletionSource<bool> completedTask;

        /// <summary>
        /// Gets a task that has already completed successfully.
        /// </summary>
        public static Task CompletedTask
        {
            get { return completedTask.Task; }
        }

        /// <summary>
        /// Creates a Task<TResult> that's completed successfully with the specified result.
        /// </summary>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>The successfully completed task.</returns>
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            #if net40
            TaskCompletionSource<TResult> proxy = new TaskCompletionSource<TResult>(result);
            proxy.SetResult(result);

            return proxy.Task;
            #else
            return Task.FromResult<TResult>(result);
            #endif
        }

        /// <summary>
        /// Creates a cancellable task that completes after a specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">
        /// The number of milliseconds to wait before completing the returned task, or -1 to 
        /// wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the time delay.</returns>
        public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
            #if net40
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }
            if (cancellationToken.IsCancellationRequested)
            {
                return canceledTask.Task;
            }
            if (millisecondsDelay == 0)
            {
                return completedTask.Task;
            }
            TaskCompletionSource<bool> promise = new TaskCompletionSource<bool>();
            CancellationTokenRegistration registry = new CancellationTokenRegistration();
            Timer timer = null;
            timer = new Timer((state) =>
            {
                registry.Dispose();
                timer.Dispose();
                promise.TrySetResult(true);

            }, null, Timeout.Infinite, Timeout.Infinite);
            if (cancellationToken.CanBeCanceled)
            {
                registry = cancellationToken.Register(() =>
                {
                    timer.Dispose();
                    promise.TrySetCanceled();

                });
            }
            timer.Change(millisecondsDelay, Timeout.Infinite);
            return promise.Task;
            #else
            return Task.Delay(millisecondsDelay, cancellationToken);
            #endif
        }
        /// <summary>
        /// Creates a cancellable task that completes after a specified time interval.
        /// </summary>
        /// <param name="delay">
        /// The time span to wait before completing the returned task, or TimeSpan.FromMilliseconds(-1)
        /// to wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the time delay.</returns>
        public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            #if net40
            long timeoutMs = (long)delay.TotalMilliseconds;
            if (timeoutMs < Timeout.Infinite || timeoutMs > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("delay");
            }
            return Delay((int)timeoutMs, cancellationToken);
            #else
            return Task.Delay(delay, cancellationToken);
            #endif
        }
        /// <summary>
        /// Creates a task that completes after a specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">
        /// The number of milliseconds to wait before completing the returned task, or -1
        /// to wait indefinitely.
        /// </param>
        /// <returns>A task that represents the time delay.</returns>
        public static Task Delay(int millisecondsDelay)
        {
            #if net40
            return Delay(millisecondsDelay, CancellationToken.None);
            #else
            return Task.Delay(millisecondsDelay);
            #endif
        }
        /// <summary>
        /// Creates a task that completes after a specified time interval.
        /// </summary>
        /// <param name="delay">
        /// The time span to wait before completing the returned task, or TimeSpan.FromMilliseconds(-1)
        /// to wait indefinitely.
        /// </param>
        /// <returns>A task that represents the time delay.</returns>
        public static Task Delay(TimeSpan delay)
        {
            #if net40
            return Delay(delay, CancellationToken.None);
            #else
            return Task.Delay(delay);
            #endif
        }

        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a Task object that represents that work. 
        /// A cancellation token allows the work to be cancelled.
        /// </summary>
        /// <param name="action">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns>A task that represents the work queued to execute in the thread pool.</returns>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            #if net40
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
            #else
            return Task.Run(action, cancellationToken);
            #endif
        }
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a proxy for the task returned by function.
        /// </summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work.</param>
        /// <returns>A task that represents a proxy for the task returned by function.</returns>
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            #if net40
            return Run<Task>(function, cancellationToken).Unwrap();
            #else
            return Task.Run(function, cancellationToken);
            #endif
        }
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a proxy for the task returned by function.
        /// </summary>
        /// <param name="function">The work to execute asynchronously</param>
        /// <returns>A task that represents a proxy for the task returned by function.</returns>
        public static Task Run(Func<Task> function)
        {
            #if net40
            return Run(function, CancellationToken.None);
            #else
            return Task.Run(function);
            #endif
        }
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a Task object that represents 
        /// that work.
        /// </summary>
        /// <param name="action">The work to execute asynchronously</param>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        public static Task Run(Action action)
        {
            #if net40
            return Run(action, CancellationToken.None);
            #else
            return Task.Run(action);
            #endif
        }

        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a Task(TResult) object that represents
        /// that work. A cancellation token allows the work to be cancelled.
        /// </summary>
        /// <param name="function">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work</param>
        /// <returns>A Task(TResult) that represents the work queued to execute in the thread pool.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            #if net40
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
            #else
            return Task.Run<TResult>(function, cancellationToken);
            #endif
        }
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a proxy for the Task(TResult) returned by function.
        /// </summary>
        /// <param name="function">The work to execute asynchronously</param>
        /// <param name="cancellationToken">A cancellation token that should be used to cancel the work</param>
        /// <returns>A Task(TResult) that represents a proxy for the Task(TResult) returned by function.</returns>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            #if net40
            return Run<Task<TResult>>(function, cancellationToken).Unwrap();
            #else
            return Task.Run<TResult>(function, cancellationToken);
            #endif
        }
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a proxy for the Task(TResult) returned by function.
        /// </summary>
        /// <param name="function">The work to execute asynchronously</param>
        /// <returns>A Task(TResult) that represents a proxy for the Task(TResult) returned by function.</returns>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            #if net40
            return Run(function, CancellationToken.None);
            #else
            return Task.Run<TResult>(function);
            #endif
        }
        /// <summary>
        /// Queues the specified work to run on the thread pool and returns a Task<TResult> object that represents that work.
        /// </summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <returns>A task object that represents the work queued to execute in the thread pool.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            #if net40
            return Run(function, CancellationToken.None);
            #else
            return Task.Run<TResult>(function);
            #endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the Task objects in an enumerable collection have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
            #if net40
            return WhenAll(tasks, (Action<Task[], TaskCompletionSource<object>>)((completedTasks, result) =>
            {
                result.TrySetResult(null);
            }));
            #else
            return Task.WhenAll(tasks);
            #endif
        }
        /// <summary>
        /// Creates a task that will complete when all of the Task objects in an array have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task WhenAll(params Task[] tasks)
        {
            #if net40
            return WhenAll(tasks.AsEnumerable());
            #else
            return Task.WhenAll(tasks);
            #endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the Task<TResult> objects in an enumerable 
        /// collection have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            #if net40
            return WhenAll<TResult[]>(tasks.Cast<Task>(), (completedTasks, tcs) =>
            {
                tcs.TrySetResult(completedTasks.Cast<Task<TResult>>()
                                               .Select(t => t.Result)
                                               .ToArray());
            });
            #else
            return Task.WhenAll<TResult>(tasks);
            #endif
        }
        /// <summary>
        /// Creates a task that will complete when all of the Task<TResult> objects in an array have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            #if net40
            return WhenAll(tasks);
            #else
            return Task.WhenAll<TResult>(tasks);
            #endif
        }

        #if net40
        private static Task<TResult> WhenAll<TResult>(IEnumerable<Task> tasks, Action<Task[], TaskCompletionSource<TResult>> setResultAction)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            TaskCompletionSource<TResult> result = new TaskCompletionSource<TResult>();
            Task[] array = tasks as Task[];
            if (array == null)
            {
                array = tasks.ToArray();
            }
            if (array.Length == 0)
            {
                setResultAction(array, result);
            }
            else Task.Factory.ContinueWhenAll(array, completedTasks =>
            {
                List<Exception> exceptions = null;
                var canceled = false;
                foreach (var task in completedTasks)
                {
                    if (task.IsFaulted)
                    {
                        AggregateException aggregateException = task.Exception as AggregateException;
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }
                        if (aggregateException != null)
                        {
                            exceptions.Add(aggregateException.InnerExceptions.Count == 1 ? task.Exception.InnerException : task.Exception);
                        }
                        else exceptions.Add(task.Exception);
                    }
                    else if (task.IsCanceled)
                        canceled = true;
                }
                if (exceptions != null && exceptions.Count > 0)
                {
                    result.TrySetException(exceptions);
                }
                else if (canceled)
                {
                    result.TrySetCanceled();
                }
                else setResultAction(completedTasks, result);

            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return result.Task;
        }
        #endif

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        /// A task that represents the completion of one of the supplied tasks. The return
        /// task's Result is the task that completed.
        /// </returns>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
            #if net40
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            TaskCompletionSource<Task> result = new TaskCompletionSource<Task>();
            Task[] array = tasks as Task[];
            if (array == null)
            {
                array = tasks.ToArray();
            }
            Task.Factory.ContinueWhenAny<bool>(array, result.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return result.Task;
            #else
            return Task.WhenAny(tasks);
            #endif
        }
        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        /// A task that represents the completion of one of the supplied tasks. The return
        /// task's Result is the task that completed.
        /// </returns>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            #if net40
            return WhenAny((IEnumerable<Task>)tasks);
            #else
            return Task.WhenAny(tasks);
            #endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        /// A task that represents the completion of one of the supplied tasks. The return
        /// task's Result is the task that completed.
        /// </returns>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            #if net40
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            TaskCompletionSource<Task<TResult>> result = new TaskCompletionSource<Task<TResult>>();
            Task<TResult>[] array = tasks as Task<TResult>[];
            if (array == null)
            {
                array = tasks.ToArray();
            }
            Task.Factory.ContinueWhenAny<TResult, bool>(array, result.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return result.Task;
            #else
            return Task.WhenAny<TResult>(tasks);
            #endif
        }
        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        /// A task that represents the completion of one of the supplied tasks. The return
        /// task's Result is the task that completed.
        /// </returns>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            #if net40
            return WhenAny((IEnumerable<Task<TResult>>)tasks);
            #else
            return Task.WhenAny<TResult>(tasks);
            #endif
        }

        /// <summary>
        /// Creates an awaitable task that asynchronously yields back to the current context when awaited.
        /// </summary>
        /// <remarks>
        /// You can use await Task.Yield(); in an asynchronous method to force the method to complete asynchronously.
        /// If there is a current synchronization context (SynchronizationContext object), this will post the remainder
        /// of the method's execution back to that context. However, the context will decide how to prioritize this work
        /// relative to other work that may be pending. The synchronization context that is present on a UI thread in
        /// most UI environments will often prioritize work posted to the context higher than input and rendering work.
        /// For this reason, do not rely on await Task.Yield(); to keep a UI responsive. For more information, see the
        /// entry Useful Abstractions Enabled with ContinueWith in the Parallel Programming with .NET blog.
        /// </remarks>
        /// <returns>
        /// A context that, when awaited, will asynchronously transition back into the current context at the time
        /// of the await. If the current SynchronizationContext is non-null, it is treated as the current context.
        /// Otherwise, the task scheduler that is associated with the currently executing task is treated as the
        /// current context.
        /// </returns>
        public static YieldAwaitable Yield()
        {
            #if net40
            return new YieldAwaitable();
            #else
            return Task.Yield();
            #endif
        }
    }
}