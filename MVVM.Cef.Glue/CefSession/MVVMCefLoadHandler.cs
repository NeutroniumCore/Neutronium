using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefSession
{
    public class MVVMCefLoadHandler : CefLoadHandler
    {
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
        }

        //protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        //{
        //}

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
        }
    }
}
