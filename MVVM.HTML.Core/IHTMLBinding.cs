using System;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core
{
    public interface IHTMLBinding : IDisposable
    {
        IWebView Context { get; }

        IJavascriptSessionInjector JavascriptUIFramework { get; }

        IJavascriptObject JSRootObject { get; }

        object Root { get; }
    }
}
