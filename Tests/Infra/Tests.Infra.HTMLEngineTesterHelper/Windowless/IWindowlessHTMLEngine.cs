using System;
using MVVM.HTML.Core;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

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
