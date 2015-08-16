using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;
using MVVM.CEFGlue.CefGlueHelper;
using System;

namespace MVVM.CEFGlue
{
    public interface IHTMLBinding : IDisposable
    {
        IWebView Context { get; }

        IJavascriptObject JSRootObject { get; }

        object Root { get; }
    }
}
