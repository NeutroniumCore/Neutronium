using Awesomium.Core;
using System;
using System.Threading.Tasks;

namespace MVVM.Awesomium
{
    public static class AwesomiumHelper
    {
        private static Task<T> EvaluateSafeAsync<T>(this IWebView iwb, Func<T> evaluate)
        {
            var tcs = new TaskCompletionSource<T>();
            WebCore.QueueWork(iwb, () => tcs.SetResult(evaluate()));
            return tcs.Task;
        }

        public static T EvaluateSafe<T>(this IWebView iwb, Func<T> evaluate)
        {
            try
            {
                return evaluate();
            }
            catch (InvalidOperationException)
            {
                return iwb.EvaluateSafeAsync(evaluate).Result;
            }
        }

        public static void ExecuteWhenReady(this IWebView view, Action toBeApplied)
        {
            new ViewReadyExecuter(view, toBeApplied).Do();
        }
    }
}
