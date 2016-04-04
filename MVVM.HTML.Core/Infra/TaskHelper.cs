using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Infra
{
    public static class TaskHelper
    {
        public static Task Ended()
        {
            return Task.FromResult<object>(null);
        }

        public static Task WaitWith<T>(this Task<T> @this, Task other, Action<Task<T>> ithen, TaskScheduler tsc)
        {
            var tasks = new[] { @this, other };
            return Task.Factory.ContinueWhenAll(tasks, (ts) => ithen(ts[0] as Task<T>), CancellationToken.None, TaskContinuationOptions.None, tsc);
        }

        public static Task WaitWith<T>(this Task<T> @this, Task other, Action<Task<T>> ithen)
        {
            return @this.WaitWith(other, ithen, TaskScheduler.FromCurrentSynchronizationContext());
        }      

        public static void DoNotWait(this Task task)
        {
            task.ContinueWith( t => Trace.WriteLine(string.Format("Exception during task execution: {0}", t.Exception))
                , TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
