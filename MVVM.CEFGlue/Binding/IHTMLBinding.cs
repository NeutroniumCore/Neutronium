using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;
using MVVM.CEFGlue.CefGlueHelper;
using System;
using Xilium.CefGlue;

namespace MVVM.CEFGlue
{
    public interface IHTMLBinding : IDisposable
    {
        IWebView Context { get; }

        CefV8Value JSRootObject { get; }

        object Root { get; }
    }
}
