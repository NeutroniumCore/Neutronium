using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSCSMappedBridge : IJSCSCachableGlue
    {
        void SetMappedJSValue(IJavascriptObject jsobject);
    }
}
