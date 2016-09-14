using System.Collections.Generic;
using System.Threading.Tasks;
using HTMLEngine.CefGlue.WindowImplementation;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.CefGlue.CefGlueImplementation;
using Neutronium.WebBrowserEngine.CefGlue.WindowImplementation;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefSession
{
    public class NeutroniumCefApp : CefApp
    {
        private readonly NeutroniumRenderProcessHandler _neutroniumRenderProcessHandler;
        private readonly NeutroniumLoadHandler _neutroniumLoadHandler;
        private readonly IDictionary<long, IWebView> _Associated = new Dictionary<long, IWebView>();
        private readonly IDictionary<long, WpfCefBrowser> _Browsers = new Dictionary<long, WpfCefBrowser>();
        private readonly IDictionary<long, TaskCompletionSource<IWebView>> _TaskCompletionSources
            = new Dictionary<long, TaskCompletionSource<IWebView>>();

        internal NeutroniumCefApp()
        {
            _neutroniumLoadHandler = new NeutroniumLoadHandler(this);
            _neutroniumRenderProcessHandler = new NeutroniumRenderProcessHandler(this, _neutroniumLoadHandler);
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
            return _neutroniumRenderProcessHandler;
        }
    }
}
