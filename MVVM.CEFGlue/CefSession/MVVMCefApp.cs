using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

using MVVM.CEFGlue.CefSession;
using MVVM.CEFGlue.CefGlueHelper;


namespace MVVM.CEFGlue.CefSession
{
    public class MVVMCefApp : CefApp
    {
        private MVVMCefRenderProcessHandler _MVVMCefRenderProcessHandler;
        private MVVMCefLoadHandler _MVVMCefLoadHandler;
        private Dictionary<long, CefV8CompleteContext> _Associated
            = new Dictionary<long, CefV8CompleteContext>();

        internal MVVMCefApp()
        {
            _MVVMCefLoadHandler = new MVVMCefLoadHandler();
            _MVVMCefRenderProcessHandler = new MVVMCefRenderProcessHandler(this, _MVVMCefLoadHandler);
        }

        internal void Associate(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _Associated.Add(frame.Identifier, new CefV8CompleteContext(context, context.GetTaskRunner()));
        }

        internal CefV8CompleteContext GetContext(CefFrame frame)
        {
            CefV8CompleteContext res = null;
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
