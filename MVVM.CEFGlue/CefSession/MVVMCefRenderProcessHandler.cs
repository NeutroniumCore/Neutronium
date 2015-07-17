using MVVM.CEFGlue.CefGlueHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefSession
{
    internal class MVVMCefRenderProcessHandler : CefRenderProcessHandler
    {
        private CefLoadHandler _CefLoadHandler;
        private MVVMCefApp _MVVMCefApp;
        internal MVVMCefRenderProcessHandler(MVVMCefApp iMVVMCefApp, CefLoadHandler iCefLoadHandler)
        {
            _MVVMCefApp = iMVVMCefApp;
            _CefLoadHandler = iCefLoadHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _CefLoadHandler;
        }


        protected override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigation_type, bool isRedirect)
        {
            return false;
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _MVVMCefApp.Associate(browser, frame, context);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
        }

        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
        }
    }
}
