using System.Threading;
using System.Threading.Tasks;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public static class TaskExtensions
    {
        public static Task<T> AsTask<T>(this CancellationToken token)
        {
            var tcs = new TaskCompletionSource<T>();
            token.Register(() => tcs.TrySetCanceled(token));
            return tcs.Task;
        }

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token, bool configureAwait= true)
        {
            return await await Task.WhenAny(task, token.AsTask<T>()).ConfigureAwait(configureAwait);
        }
    }
}