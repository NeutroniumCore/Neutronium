using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Windows.Input;
using Neutronium.Core.Extension;

namespace Neutronium.Core.Binding.GlueObject.Mapped
{
    internal class JSMappableCommand: JSCommand, IJSCSMappedBridge
    {
        private IJavascriptObject _MappedJSValue;

        public override IJavascriptObject CachableJSValue => _MappedJSValue;

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
            _MappedJSValue.Bind("Execute", WebView, ExecuteCommand);
            _MappedJSValue.Bind("CanExecute", WebView, CanExecuteCommand);
        }

        public JSMappableCommand(HTMLViewContext context, IJavascriptToCSharpConverter converter, ICommand command):
            base(context, converter, command)
        {
        }
    }
}
