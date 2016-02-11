using System.Windows.Input;
using MVVM.Component;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    internal interface IJSCommandFactory
    {
        JSCommand Build(IWebView view, IDispatcher uiDispatcher, ICommand command);

        JSSimpleCommand Build(IWebView view, IDispatcher uiDispatcher, ISimpleCommand command);

        JSResultCommand Build(IWebView view, IDispatcher uiDispatcher, IResultCommand command);
    }
}
