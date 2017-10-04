using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    /// <summary>
    /// ISimpleCommand implmentation based on an action taking one argument
    /// <seealso cref="ISimpleCommand"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelaySimpleCommand<T> : ISimpleCommand<T>, ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }

        private readonly Action<T> _Do;

        public RelaySimpleCommand(Action<T> doAction)
        {
            _Do = doAction;
        }

        public bool CanExecute(object parameter) => true;

        [DebuggerStepThrough]
        public void Execute(T argument)
        {
            _Do(argument);
        }

        [DebuggerStepThrough]
        public void Execute(object parameter) 
        {
            if (parameter is T)
                Execute((T)parameter);
        }
    }
}
