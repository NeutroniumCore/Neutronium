using System.Diagnostics;
using System.Threading.Tasks;

namespace Neutronium.Core.Infra
{
    public static class TaskHelper
    {
        public static Task Ended()
        {
            return Task.FromResult<object>(null);
        }

        public static async Task<T> WaitWith<T>(this Task<T> @this, Task other)
        {
            var tasks = new[] { @this, other };
            await Task.WhenAll(tasks);
            return @this.Result;
        }

        public static void DoNotWait(this Task task)
        {
            task.ContinueWith( t => Trace.WriteLine($"Exception during task execution: {t.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
