using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJsCsCachableGlue : IJsCsGlue
    {
        IJavascriptObject CachableJsValue { get; }

        void SetJsId(uint jsId);
    }
}
