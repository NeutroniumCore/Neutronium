using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Infra
{
    public static class TaskHelper
    {
        public static Task<T> FromResult<T>(T result)
        {
            TaskCompletionSource<T> res = new TaskCompletionSource<T>();
            res.SetResult(result);
            return res.Task;
        }

        public static Task Ended()
        {
            return FromResult<object>(null);
        }

        public static Task WaitWith<T>(this Task<T> @this, Task other, Action<Task<T>> ithen, TaskScheduler tsc)
        {
            Task[] tasks = new Task[2] { @this, other };
            return Task.Factory.ContinueWhenAll(tasks, (ts) => ithen(ts[0] as Task<T>), CancellationToken.None, TaskContinuationOptions.None, tsc);
        }

        public static Task WaitWith<T>(this Task<T> @this, Task other, Action<Task<T>> ithen)
        {
            return @this.WaitWith(other, ithen, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
    }
}
