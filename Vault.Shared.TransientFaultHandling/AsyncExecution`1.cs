using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vault.Shared.TransientFaultHandling
{
    /// <summary>
    /// Handles the execution and retries of the user-initiated task.
    /// </summary>
    /// <typeparam name="TResult">The result type of the user-initiated task.</typeparam>
    internal class AsyncExecution<TResult>
    {
        public AsyncExecution(Func<Task<TResult>> taskFunc, ShouldRetry shouldRetry, Func<Exception, bool> isTransient, Action<int, Exception, TimeSpan> onRetrying, bool fastFirstRetry, CancellationToken cancellationToken)
        {
            this.taskFunc = taskFunc;
            this.shouldRetry = shouldRetry;
            this.isTransient = isTransient;
            this.onRetrying = onRetrying;
            this.fastFirstRetry = fastFirstRetry;
            this.cancellationToken = cancellationToken;
        }

        internal Task<TResult> ExecuteAsync()
        {
            return this.ExecuteAsyncImpl(null);
        }

        private Task<TResult> ExecuteAsyncContinueWith(Task<TResult> runningTask)
        {
            if (!runningTask.IsFaulted || this.cancellationToken.IsCancellationRequested)
            {
                return runningTask;
            }
            TimeSpan zero = TimeSpan.Zero;
            Exception innerException = runningTask.Exception.InnerException;
            if (innerException is RetryLimitExceededException)
            {
                TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
                if (innerException.InnerException != null)
                {
                    taskCompletionSource.TrySetException(innerException.InnerException);
                }
                else
                {
                    taskCompletionSource.TrySetCanceled();
                }
                return taskCompletionSource.Task;
            }
            if (!this.isTransient(innerException) || !this.shouldRetry(this.retryCount++, innerException, out zero))
            {
                return runningTask;
            }
            if (zero < TimeSpan.Zero)
            {
                zero = TimeSpan.Zero;
            }
            this.onRetrying(this.retryCount, innerException, zero);
            this.previousTask = runningTask;
            if (zero > TimeSpan.Zero && (this.retryCount > 1 || !this.fastFirstRetry))
            {
                return Task.Delay(zero).ContinueWith<Task<TResult>>(new Func<Task, Task<TResult>>(this.ExecuteAsyncImpl), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Unwrap<TResult>();
            }
            return this.ExecuteAsyncImpl(null);
        }

        private Task<TResult> ExecuteAsyncImpl(Task ignore)
        {
            if (this.cancellationToken.IsCancellationRequested)
            {
                if (this.previousTask != null)
                {
                    return this.previousTask;
                }
                TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
                taskCompletionSource.TrySetCanceled();
                return taskCompletionSource.Task;
            }
            else
            {
                Task<TResult> task;
                try
                {
                    task = this.taskFunc();
                }
                catch (Exception ex)
                {
                    if (!this.isTransient(ex))
                    {
                        throw;
                    }
                    TaskCompletionSource<TResult> taskCompletionSource2 = new TaskCompletionSource<TResult>();
                    taskCompletionSource2.TrySetException(ex);
                    task = taskCompletionSource2.Task;
                }
                if (task == null)
                {
                    throw new ArgumentException("Task cannot be null");
                }
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return task;
                }
                if (task.Status == TaskStatus.Created)
                {
                    throw new ArgumentException("Task must be scheduled");
                }
                return task.ContinueWith(new Func<Task<TResult>, Task<TResult>>(this.ExecuteAsyncContinueWith), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Unwrap<TResult>();
            }
        }

        private readonly CancellationToken cancellationToken;

        private readonly bool fastFirstRetry;

        private readonly Func<Exception, bool> isTransient;

        private readonly Action<int, Exception, TimeSpan> onRetrying;

        private Task<TResult> previousTask;

        private int retryCount;

        private readonly ShouldRetry shouldRetry;

        private readonly Func<Task<TResult>> taskFunc;
    }
}