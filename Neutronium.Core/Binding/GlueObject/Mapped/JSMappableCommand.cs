using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.Mapper;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal sealed class JsMappableCommand: JsCommand, IJsCsMappedBridge
    {
        private IJavascriptObject _MappedJsValue;

        public override IJavascriptObject CachableJsValue => _MappedJsValue;

        public override void SetJsValue(IJavascriptObject value, ISessionCache sessionCache)
        {
            SetJsValue(value);
        }

        public void SetMappedJsValue(IJavascriptObject jsobject)
        {
            _MappedJsValue = jsobject;
            UpdateJsObject(_MappedJsValue);
        }

        public JsMappableCommand(HtmlViewContext context, IJavascriptToGlueMapper converter, ICommand command):
            base(context, converter, command)
        {
        }
    }
}
