using System;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    /// <summary>
    /// ISimpleCommand implementation based on action with no argument
    /// <seealso cref="ISimpleCommand"/>
    /// </summary>
    public class RelaySimpleCommand : ICommand, ISimpleCommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action _Do;

        public RelaySimpleCommand(Action doAction)
        {
            _Do = doAction;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute() => _Do();

        public void Execute(object parameter) => Execute();
    }
}
