// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
    public static partial class TaskExtension
    {
        #if net40
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return task.ContinueWith((parent) =>
            {
                continuationAction(parent, state);

            }, cancellationToken, continuationOptions, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(task, continuationAction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return task.ContinueWith((parent) =>
            {
                continuationAction(parent, state);

            }, cancellationToken, continuationOptions, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith<TResult>(task, continuationAction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith<TResult>(task, continuationAction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
        {
            return ContinueWith<TResult>(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationAction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state)
        {
            return ContinueWith<TResult>(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return task.ContinueWith((parent) =>
            {
                return continuationFunction(parent, state);

            }, cancellationToken, continuationOptions, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith<TResult>(task, continuationFunction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith<TResult>(task, continuationFunction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            return ContinueWith<TResult>(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state)
        {
            return ContinueWith<TResult>(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<T, TResult>(this Task<T> task, Func<Task<T>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return task.ContinueWith((parent) =>
            {
                return continuationFunction(parent, state);

            }, cancellationToken, continuationOptions, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="cancellationToken">The CancellationToken that will be assigned to the new continuation task.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<T, TResult>(this Task<T> task, Func<Task<T>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith<T, TResult>(task, continuationFunction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="continuationOptions">
        /// Options for when the continuation is scheduled and how it behaves. This includes criteria, such as OnlyOnCanceled, 
        /// as well as execution options, such as ExecuteSynchronously.
        /// </param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<T, TResult>(this Task<T> task, Func<Task<T>, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith<T, TResult>(task, continuationFunction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <param name="scheduler">The TaskScheduler to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<T, TResult>(this Task<T> task, Func<Task<T>, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            return ContinueWith<T, TResult>(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }
        /// <summary>
        /// Creates a continuation that receives caller-supplied state information and a cancellation token and that 
        /// executes when the target Task completes. The continuation executes based on a set of specified conditions 
        /// and uses a specified scheduler.
        /// </summary>
        /// <param name="continuationFunction">
        /// An action to run when the Task completes. When run, the delegate will be passed the completed task and the 
        /// caller-supplied state object as arguments.
        /// </param>
        /// <param name="state">An object representing data to be used by the continuation action.</param>
        /// <returns>A new continuation Task.</returns>
        public static Task<TResult> ContinueWith<T, TResult>(this Task<T> task, Func<Task<T>, object, TResult> continuationFunction, object state)
        {
            return ContinueWith<T, TResult>(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }
        #endif
    }
}
