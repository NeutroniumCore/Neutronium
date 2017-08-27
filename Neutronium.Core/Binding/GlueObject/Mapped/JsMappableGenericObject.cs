using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableGenericObject : JsGenericObject, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJSValue;
        public override IJavascriptObject CachableJsValue => _MappedJSValue;

        public JsMappableGenericObject(object cValue, int childrenCount): base (cValue, childrenCount)
        {
        }

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
        }
    }
}
