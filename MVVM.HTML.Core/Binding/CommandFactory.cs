using System.Windows.Input;
using MVVM.Component;
using MVVM.HTML.Core.Binding.GlueObject;

namespace MVVM.HTML.Core.Binding
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

        public JSSimpleCommand Build(ISimpleCommand command)
        {
            return new JSSimpleCommand(_HTMLViewContext.WebView, _JavascriptToCSharpConverter, command);
        }

        public JSResultCommand Build(IResultCommand command)
        {
            return new JSResultCommand(_HTMLViewContext.WebView, _JavascriptToCSharpConverter, command);
        }
    }
}
