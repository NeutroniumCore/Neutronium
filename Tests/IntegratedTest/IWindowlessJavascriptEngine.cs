using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System;

namespace IntegratedTest
{
    public interface IWindowlessJavascriptEngine : IDisposable
    {
        void Init(string path = "javascript\\index.html");
        HTMLViewEngine ViewEngine { get; }
        IWebView WebView { get; }
    }
}
