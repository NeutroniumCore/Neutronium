using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSCSCachableGlue : IJSCSGlue
    {
        IJavascriptObject CachableJSValue { get; }

        void SetJsId(uint jsId);
    }
}
