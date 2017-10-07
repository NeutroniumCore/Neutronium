using System;

namespace Neutronium.MVVMComponents
{
    public interface IUpdatableCommand
    {
        ///<summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        event EventHandler CanExecuteChanged;
    }
}
