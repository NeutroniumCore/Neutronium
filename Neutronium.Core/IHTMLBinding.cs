using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core
{
    public interface IHTMLBinding : IDisposable
    {
        JavascriptBindingMode Mode { get; }
        IWebView Context { get; }
        IJavascriptSessionInjector JavascriptUIFramework { get; }
        IJsCsGlue JSBrideRootObject { get; }
        IJavascriptObject JSRootObject { get; }
        object Root { get; }
    }
}
