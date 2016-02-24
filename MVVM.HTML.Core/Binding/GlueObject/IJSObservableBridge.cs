using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    public interface IJSObservableBridge : IJSCSGlue
    {
        IJavascriptObject MappedJSValue { get; }
        void SetMappedJSValue(IJavascriptObject ijsobject);
    }
}
