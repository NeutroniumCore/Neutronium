using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    interface ICfxJavascriptObject : IJavascriptObject
    {
        CfrV8Value GetRaw();
    }
}
