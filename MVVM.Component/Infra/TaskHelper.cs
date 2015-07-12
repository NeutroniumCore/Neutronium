using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Component.Infra
{
    public static class TaskHelper
    {
        public static Task<object> Convert<T>(this Task<T> task)
        {
            TaskCompletionSource<object> res = new TaskCompletionSource<object>();

            return task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    res.TrySetCanceled();
                }
                else if (t.IsFaulted)
                {
                    res.TrySetException(t.Exception);
                }
                else
                {
                    res.TrySetResult(t.Result);
                }
                return res.Task;
            }
            , TaskContinuationOptions.ExecuteSynchronously).Unwrap();
        }
    }
}
