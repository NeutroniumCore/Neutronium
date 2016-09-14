using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Binding.GlueObject
{
    internal interface IJSCommandFactory
    {
        JSCommand Build(ICommand command);

        JsSimpleCommand Build(ISimpleCommand command);

        JsResultCommand Build(IResultCommand command);
    }
}
