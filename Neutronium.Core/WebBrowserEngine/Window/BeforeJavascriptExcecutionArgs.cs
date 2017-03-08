using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class BeforeJavascriptExcecutionArgs : EventArgs
    {
        public Action<string> JavascriptExecutor { get; }
        public IWebView WebView { get; }

        public BeforeJavascriptExcecutionArgs(IWebView webview, Action<string> javascriptExecutor)
        {
            JavascriptExecutor = javascriptExecutor;
            WebView = webview;
        }
    }
}
