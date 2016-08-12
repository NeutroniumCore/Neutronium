using System;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace IntegratedTest.Infra.Windowless
{
    public interface IWindowlessJavascriptEngine : IDisposable 
    {
        void Init(string path);
        HTMLViewEngine ViewEngine { get; }
        IWebView WebView { get; }
    }
}
