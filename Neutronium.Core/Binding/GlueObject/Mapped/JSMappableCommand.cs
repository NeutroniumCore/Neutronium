using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Windows.Input;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JSMappableCommand: JSCommand, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJSValue;

        public override IJavascriptObject CachableJsValue => _MappedJSValue;

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
            UpdateJsObject(_MappedJSValue);
        }

        public JSMappableCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, ICommand command):
            base(context, converter, command)
        {
        }
    }
}
