using System.Windows.Input;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding
{
    internal class CommandFactory : IJSCommandFactory
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        private readonly HTMLViewContext _HTMLViewContext;

        public CommandFactory(HTMLViewContext context, IJavascriptToCSharpConverter converter)
        {
            _HTMLViewContext = context;
            _JavascriptToCSharpConverter = converter;
        }

        public JSCommand Build(ICommand command)
        {
            return new JSCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsSimpleCommand Build(ISimpleCommand command)
        {
            return new JsSimpleCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }

        public JsResultCommand Build(IResultCommand command)
        {
            return new JsResultCommand(_HTMLViewContext, _JavascriptToCSharpConverter, command);
        }
    }
}
