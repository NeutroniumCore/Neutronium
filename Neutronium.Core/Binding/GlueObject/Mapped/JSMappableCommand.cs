using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Windows.Input;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JsMappableCommand: JsCommand, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;

        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {
            _MappedJsValue = jsobject;
            UpdateJsObject(_MappedJsValue);
        }

        public JsMappableCommand(HtmlViewContext context, IJavascriptToCSharpConverter converter, ICommand command):
            base(context, converter, command)
        {
        }
    }
}
