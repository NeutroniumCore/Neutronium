using Neutronium.Core.Infra.Reflection;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableGenericObject : JsGenericObject, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;
        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public JsMappableGenericObject(object cValue, TypePropertyAccessor typePropertyAccessor) : 
            base (cValue, typePropertyAccessor)
        {
        }

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {
            _MappedJsValue = jsobject;
        }
    }
}
