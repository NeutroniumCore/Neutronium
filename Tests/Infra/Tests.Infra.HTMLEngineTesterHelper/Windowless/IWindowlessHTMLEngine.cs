using System;
using Neutronium.Core;
using Neutronium.Core.JavascriptEngine.Control;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptEngine.Window;

namespace Tests.Infra.HTMLEngineTesterHelper.Windowless
{
    public interface IWindowlessHTMLEngine : IDisposable 
    {
        void Init(string path, IWebSessionLogger logger);
        IWebView WebView { get; }
        IHTMLWindow HTMLWindow { get; }
        IHTMLWindowProvider HTMLWindowProvider {get;}
    }
}
