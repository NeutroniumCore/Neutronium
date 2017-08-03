using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    public class JsMappableSimpleCommand: JsSimpleCommand, IJSCSMappedBridge
    {
        private IJavascriptObject _MappedJSValue;

        public override IJavascriptObject CachableJSValue => _MappedJSValue;

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
            UpdateJsObject(_MappedJSValue);
        }

        public JsMappableSimpleCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand resultCommand)
            :base(context, converter, resultCommand)
        {
        }
    }
}
