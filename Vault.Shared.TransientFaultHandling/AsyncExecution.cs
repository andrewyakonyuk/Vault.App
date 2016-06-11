using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Shared.TransientFaultHandling
{
    /// <summary>
    /// Provides a wrapper for a non-generic <see cref="T:System.Threading.Tasks.Task" /> and calls into the pipeline
    /// to retry only the generic version of the <see cref="T:System.Threading.Tasks.Task" />.
    /// </summary>
    internal class AsyncExecution : AsyncExecution<bool>
    {
        public AsyncExecution(Func<Task> taskAction, ShouldRetry shouldRetry, Func<Exception, bool> isTransient, Action<int, Exception, TimeSpan> onRetrying, bool fastFirstRetry, CancellationToken cancellationToken) : base(() => AsyncExecution.StartAsGenericTask(taskAction), shouldRetry, isTransient, onRetrying, fastFirstRetry, cancellationToken)
        {
        }

        private static Task<bool> GetCachedTask()
        {
            if (AsyncExecution.cachedBoolTask == null)
            {
                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.TrySetResult(true);
                AsyncExecution.cachedBoolTask = taskCompletionSource.Task;
            }
            return AsyncExecution.cachedBoolTask;
        }

        /// <summary>
        /// Wraps the non-generic <see cref="T:System.Threading.Tasks.Task" /> into a generic <see cref="T:System.Threading.Tasks.Task" />.
        /// </summary>
        /// <param name="taskAction">The task to wrap.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that wraps the non-generic <see cref="T:System.Threading.Tasks.Task" />.</returns>
        private static Task<bool> StartAsGenericTask(Func<Task> taskAction)
        {
            Task task = taskAction();
            if (task == null)
            {
                throw new ArgumentException("Task cannot be null");
            }
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return AsyncExecution.GetCachedTask();
            }
            if (task.Status == TaskStatus.Created)
            {
                throw new ArgumentException("Task must be scheduled");
            }
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            task.ContinueWith(delegate (Task t)
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.InnerExceptions);
                    return;
                }
                if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                    return;
                }
                tcs.TrySetResult(true);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        private static Task<bool> cachedBoolTask;
    }
}