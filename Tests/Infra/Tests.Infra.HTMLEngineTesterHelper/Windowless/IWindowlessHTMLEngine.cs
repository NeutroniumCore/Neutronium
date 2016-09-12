using System;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Tests.Infra.HTMLEngineTesterHelper.Windowless
{
    public interface IWindowlessHTMLEngine : IDisposable 
    {
        void Init(string path, IWebSessionLogger logger);
        IWebView WebView { get; }
        IWebBrowserWindow HTMLWindow { get; }
        IWebBrowserWindowProvider HTMLWindowProvider {get;}
    }
}
