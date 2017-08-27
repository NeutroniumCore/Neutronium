using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableResultCommand : JsResultCommand, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJSValue;

        public override IJavascriptObject CachableJsValue => _MappedJSValue;

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {         
            _MappedJSValue = jsobject;
            UpdateJsObject(_MappedJSValue);
        }

        public JsMappableResultCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
            :base(context, converter, resultCommand)
        {
        }
    }
}
