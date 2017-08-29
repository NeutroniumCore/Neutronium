using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core
{
    public interface IHtmlBinding : IDisposable
    {
        JavascriptBindingMode Mode { get; }
        IWebView Context { get; }
        IJavascriptSessionInjector JavascriptUiFramework { get; }
        IJsCsGlue JsBrideRootObject { get; }
        IJavascriptObject JsRootObject { get; }
        object Root { get; }
    }
}
