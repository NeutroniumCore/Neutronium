using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Neutronium.Core.Infra
{
    public static class TaskHelper
    {
        public static Task Ended()
        {
            return Task.FromResult<object>(null);
        }

        public static Task WaitWith<T>(this Task<T> @this, Task other, Action<Task<T>> then, TaskScheduler tsc)
        {
            var tasks = new[] { @this, other };
            return Task.Factory.ContinueWhenAll(tasks, (ts) => then(ts[0] as Task<T>), CancellationToken.None, TaskContinuationOptions.None, tsc);
        }

        public static Task WaitWith<T>(this Task<T> @this, Task other, Action<Task<T>> then)
        {
            return @this.WaitWith(other, then, TaskScheduler.FromCurrentSynchronizationContext());
        }      

        public static void DoNotWait(this Task task)
        {
            task.ContinueWith( t => Trace.WriteLine($"Exception during task execution: {t.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
