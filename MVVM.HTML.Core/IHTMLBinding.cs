using System;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.HTML.Core
{
    public interface IHTMLBinding : IDisposable
    {
        JavascriptBindingMode Mode { get; }

        IWebView Context { get; }

        IJavascriptSessionInjector JavascriptUIFramework { get; }

        IJavascriptObject JSRootObject { get; }

        object Root { get; }
    }
}
