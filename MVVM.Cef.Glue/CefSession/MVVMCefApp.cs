using System.Collections.Generic;
using Xilium.CefGlue;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Infra;

namespace MVVM.Cef.Glue.CefSession
{
    public class MVVMCefApp : CefApp
    {
        private readonly MVVMCefRenderProcessHandler _MVVMCefRenderProcessHandler;
        private readonly MVVMCefLoadHandler _MVVMCefLoadHandler;
        private readonly IDictionary<long, IWebView> _Associated = new Dictionary<long, IWebView>();

        internal MVVMCefApp()
        {
            _MVVMCefLoadHandler = new MVVMCefLoadHandler();
            _MVVMCefRenderProcessHandler = new MVVMCefRenderProcessHandler(this, _MVVMCefLoadHandler);
        }

        internal void Associate(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _Associated.Add(frame.Identifier, new CefV8_WebView(context, context.GetTaskRunner()));
        }

        internal void Reset(CefFrame frame)
        {
            _Associated.Remove(frame.Identifier);
        }

        internal IWebView GetContext(CefFrame frame)
        {
            return _Associated.GetOrDefault(frame.Identifier);
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _MVVMCefRenderProcessHandler;
        }
    }
}
