using System.Windows.Input;
using MVVM.Component;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    internal interface IJSCommandFactory
    {
        JSCommand Build(ICommand command);

        JSSimpleCommand Build(ISimpleCommand command);

        JSResultCommand Build(IResultCommand command);
    }
}
