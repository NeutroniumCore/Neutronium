using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSObservableBridge : IJSCSGlue
    {
        IJavascriptObject MappedJSValue { get; }
        void SetMappedJSValue(IJavascriptObject ijsobject);
    }
}
