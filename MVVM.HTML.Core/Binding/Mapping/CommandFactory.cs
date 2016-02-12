using System;
using System.Windows.Input;
using MVVM.Component;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Binding.Mapping
{
    internal class CommandFactory : IJSCommandFactory
    {
        private readonly IJavascriptToCSharpConverter _JavascriptToCSharpConverter;
        public CommandFactory(IJavascriptToCSharpConverter converter)
        {
            _JavascriptToCSharpConverter = converter;
        }

        public JSCommand Build(IWebView view, IDispatcher uiDispatcher, ICommand command)
        {
            return new JSCommand(view, _JavascriptToCSharpConverter, uiDispatcher, command);
        }

        public JSSimpleCommand Build(IWebView view, IDispatcher uiDispatcher, ISimpleCommand command)
        {
            return new JSSimpleCommand(view, _JavascriptToCSharpConverter, command);
        }

        public JSResultCommand Build(IWebView view, IDispatcher uiDispatcher, IResultCommand command)
        {
            return new JSResultCommand(view, _JavascriptToCSharpConverter, command);
        }
    }
}
