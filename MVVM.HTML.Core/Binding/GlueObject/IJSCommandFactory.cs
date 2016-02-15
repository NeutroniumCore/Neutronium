using System.Windows.Input;
using MVVM.Component;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    internal interface IJSCommandFactory
    {
        JSCommand Build(ICommand command);

        JSSimpleCommand Build(ISimpleCommand command);

        JSResultCommand Build(IResultCommand command);
    }
}
