using System;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptFramework;

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
