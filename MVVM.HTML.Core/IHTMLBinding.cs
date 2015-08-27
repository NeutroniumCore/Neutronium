using System;

using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core
{
    public interface IHTMLBinding : IDisposable
    {
        IWebView Context { get; }

        IJavascriptObject JSRootObject { get; }

        object Root { get; }
    }
}
