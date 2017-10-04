using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayCommand<T> : ICommand, ICommand<T>
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }
        private readonly Action<T> _Execute;

        public RelayCommand(Action<T> execute)
        {
            _Execute = execute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) => true;

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            if (parameter is T)
                Execute((T)parameter);
        }

        [DebuggerStepThrough]
        public void Execute(T argument)
        {
            _Execute(argument);
        }

        public bool CanExecute(T parameter) => true;
    }
}
