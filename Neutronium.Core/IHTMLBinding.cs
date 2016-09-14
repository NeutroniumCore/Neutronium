using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core
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
