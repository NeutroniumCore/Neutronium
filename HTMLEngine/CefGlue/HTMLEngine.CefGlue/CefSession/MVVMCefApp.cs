using System.Collections.Generic;
using System.Threading.Tasks;
using HTMLEngine.CefGlue.CefGlueImplementation;
using HTMLEngine.CefGlue.WindowImplementation;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefSession
{
    public class MVVMCefApp : CefApp
    {
        private readonly MVVMCefRenderProcessHandler _MVVMCefRenderProcessHandler;
        private readonly MVVMCefLoadHandler _MVVMCefLoadHandler;
        private readonly IDictionary<long, IWebView> _Associated = new Dictionary<long, IWebView>();
        private readonly IDictionary<long, WpfCefBrowser> _Browsers = new Dictionary<long, WpfCefBrowser>();
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

        internal void Reset(CefBrowser browser)
        {
            _Browsers.Remove(browser.Identifier);
        }

        internal void Associate(CefBrowser browser, WpfCefBrowser wpfBrowser)
        {
            _Browsers.Add(browser.Identifier, wpfBrowser);
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
        }

        internal IWebView GetContext(CefFrame frame)
        {
            var taskCompletionSource = _TaskCompletionSources.GetOrDefault(frame.Identifier);
            if (taskCompletionSource != null)
                return taskCompletionSource.Task.Result;

            return _Associated.GetOrDefault(frame.Identifier);
        }

        internal void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            var wpfBrowser = _Browsers.GetOrDefault(browser.Identifier);
            if (wpfBrowser != null)
                wpfBrowser.FireFirstLoad(frame);
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _MVVMCefRenderProcessHandler;
        }
    }
}
