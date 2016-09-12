using System.Windows.Input;
using MVVM.Component;

namespace Neutronium.Core.Binding.GlueObject
{
    internal interface IJSCommandFactory
    {
        JSCommand Build(ICommand command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsResultCommand Build(IResultCommand command);
    }
}
