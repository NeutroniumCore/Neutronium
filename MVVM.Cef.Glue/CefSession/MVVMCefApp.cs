using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xilium.CefGlue;

using MVVM.HTML.Core.V8JavascriptObject;

using MVVM.Cef.Glue.CefSession;
using MVVM.Cef.Glue.CefGlueHelper;
using MVVM.Cef.Glue;



namespace MVVM.Cef.Glue.CefSession
{
    public class MVVMCefApp : CefApp
    {
        private MVVMCefRenderProcessHandler _MVVMCefRenderProcessHandler;
        private MVVMCefLoadHandler _MVVMCefLoadHandler;
        private Dictionary<long, IWebView> _Associated = new Dictionary<long, IWebView>();

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
            IWebView res = null;
            _Associated.TryGetValue(frame.Identifier, out res);
            return res;
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _MVVMCefRenderProcessHandler;
        }
    }
}
