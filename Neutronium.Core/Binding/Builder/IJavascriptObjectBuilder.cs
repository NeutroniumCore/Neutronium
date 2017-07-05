using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.Core.Builder
{
    public interface IJavascriptObjectBuilder
    {
        IJavascriptObjectFactory Factory { get; }

        IWebView WebView { get; }

        void Cache(object @object, IJSCSGlue glueObject);

        void RequestObjectCreation(Action<IJavascriptObject> afterBuild);

        void RequestArrayCreation(Action<IJavascriptObject> afterBuild);
    }
}