using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class JavascriptObjectExtension
    {
        private class TaskToPromiseHandler : ITaskProvider
        {
            public Task Task => _Tcs.Task;
            private readonly TaskCompletionSource<object> _Tcs;
            private readonly IJavascriptObject _JavascriptObject;

            public TaskToPromiseHandler(IWebView webView)
            {
                _Tcs = new TaskCompletionSource<object>();
                _JavascriptObject = webView.Factory.CreateObject();
                _JavascriptObject.BindArgument("fulfill", webView, _ =>
                {
                    _Tcs.TrySetResult(null);
                    _JavascriptObject.Dispose();
                });
            }

            public IJavascriptObject Fufill => _JavascriptObject.GetValue("fulfill");
        }

        public interface ITaskProvider
        {
            Task Task { get; }
        }

        public static ITaskProvider TransformPromiseToTask(this IJavascriptObject javascriptObjectPromise, IWebView webView)
        {
            var promiseHandler = new TaskToPromiseHandler(webView);
            javascriptObjectPromise.Invoke("then", webView, promiseHandler.Fufill);
            return promiseHandler;
        }
    }
}
