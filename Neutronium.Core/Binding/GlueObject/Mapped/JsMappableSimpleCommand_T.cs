using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal sealed class JsMappableSimpleCommand<T>: JsSimpleCommand<T>, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;

        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public override void SetJsValue(IJavascriptObject value, IJavascriptSessionCache sessionCache)
        {
            SetJsValue(value);
        }

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {
            _MappedJsValue = jsobject;
            UpdateJsObject(_MappedJsValue);
        }

        public JsMappableSimpleCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ISimpleCommand<T> resultCommand)
            :base(context, converter, resultCommand)
        {
        }
    }
}
