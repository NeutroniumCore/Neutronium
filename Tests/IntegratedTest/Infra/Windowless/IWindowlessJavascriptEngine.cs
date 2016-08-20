using System;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace IntegratedTest.Infra.Windowless
{
    public interface IWindowlessJavascriptEngine : IDisposable 
    {
        void Init(string path, IWebSessionLogger logger);
        HTMLViewEngine ViewEngine { get; }
        IWebView WebView { get; }
        IHTMLWindow HTMLWindow { get; }
    }
}
