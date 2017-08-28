using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableResultCommand : JsResultCommand, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;

        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {         
            _MappedJsValue = jsobject;
            UpdateJsObject(_MappedJsValue);
        }

        public JsMappableResultCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, IResultCommand resultCommand)
            :base(context, converter, resultCommand)
        {
        }
    }
}
