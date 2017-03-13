using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.WebBrowserEngine.Control 
{
    public class DebugEventArgs: EventArgs 
    {
        public bool Opening { get; }
        public IWebView WebView { get; }

        public DebugEventArgs(bool opening, IWebView webView) 
        {
            Opening = opening;
            WebView = webView;
        }
    }
}
