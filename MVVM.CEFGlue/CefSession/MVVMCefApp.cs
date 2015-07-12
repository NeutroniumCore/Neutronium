using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefSession
{
    public class MVVMCefApp : CefApp
    {
        private MVVMCefRenderProcessHandler _MVVMCefRenderProcessHandler;
        private MVVMCefLoadHandler _MVVMCefLoadHandler;
        private Dictionary<long, CefV8Context> _Associated
            = new Dictionary<long, CefV8Context>();

        internal MVVMCefApp()
        {
            _MVVMCefLoadHandler = new MVVMCefLoadHandler();
            _MVVMCefRenderProcessHandler = new MVVMCefRenderProcessHandler(this, _MVVMCefLoadHandler);
        }

        internal void Associate(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _Associated.Add(frame.Identifier, context);
        }

        internal CefV8Context GetContext(CefFrame frame)
        {
            CefV8Context res = null;
            _Associated.TryGetValue(frame.Identifier, out res);
            return res;
        }


        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            //return base.GetRenderProcessHandler();
            return _MVVMCefRenderProcessHandler;
        }
    }
}
