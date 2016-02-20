using System.Collections.Generic;
using Xilium.CefGlue;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System.Threading.Tasks;

namespace MVVM.Cef.Glue.CefSession
{
    public class MVVMCefApp : CefApp
    {
        private readonly MVVMCefRenderProcessHandler _MVVMCefRenderProcessHandler;
        private readonly MVVMCefLoadHandler _MVVMCefLoadHandler;
        private readonly IDictionary<long, IWebView> _Associated = new Dictionary<long, IWebView>();
        private readonly IDictionary<long, TaskCompletionSource<IWebView>> _TaskCompletionSources
            = new Dictionary<long, TaskCompletionSource<IWebView>>();

        internal MVVMCefApp()
        {
            _MVVMCefLoadHandler = new MVVMCefLoadHandler(this);
            _MVVMCefRenderProcessHandler = new MVVMCefRenderProcessHandler(this, _MVVMCefLoadHandler);
        }

        internal void Associate(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            var webView = new CefV8_WebView(context,context.GetTaskRunner());
            var taskCompletionSource =  _TaskCompletionSources.GetOrDefault(frame.Identifier);  
            if (taskCompletionSource!=null)
            {
                _TaskCompletionSources.Remove(frame.Identifier);
                taskCompletionSource.TrySetResult(webView);
            }
            _Associated.Add(frame.Identifier, webView);
          
        }

        internal void Reset(CefFrame frame)
        {
            _Associated.Remove(frame.Identifier);
        }

        internal void EnsureLoaded(CefFrame frame)
        {
            var view = GetContext(frame);
            if (view != null)
                return;
            var taskCompletionSource = new TaskCompletionSource<IWebView>();
            _TaskCompletionSources.Add(frame.Identifier, taskCompletionSource);
            //run dummy script to load context
            frame.ExecuteJavaScript("(function(){})()", string.Empty, 0);
            //taskCompletionSource.Task.Wait();
        }

        internal IWebView GetContext(CefFrame frame)
        {
            var taskCompletionSource = _TaskCompletionSources.GetOrDefault(frame.Identifier);
            if (taskCompletionSource != null)
                return taskCompletionSource.Task.Result;

            return _Associated.GetOrDefault(frame.Identifier);
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _MVVMCefRenderProcessHandler;
        }
    }
}
