using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;
using Neutronium.Core.Extension;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableResultCommand : JsResultCommand, IJSCSMappedBridge
    {
        private IJavascriptObject _MappedJSValue;

        public override IJavascriptObject CachableJSValue => _MappedJSValue;

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
            _MappedJSValue.Bind("Execute", WebView, Execute);
        }

        public JsMappableResultCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
            :base(context, converter, resultCommand)
        {
        }
    }
}
