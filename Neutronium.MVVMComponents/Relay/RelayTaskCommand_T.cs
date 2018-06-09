using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Neutronium.MVVMComponents.Helper;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayTaskCommand<T> : ICommand, ICommand<T>
    {
        public event EventHandler CanExecuteChanged;
        private readonly Func<T, Task> _Execute;
        private bool _ShouldExecute = true;

        public RelayTaskCommand(Func<T, Task> execute)
        {
            _Execute = execute;
        }

        public bool CanExecute(object parameter) => _ShouldExecute;
        public bool CanExecute(T parameter) => _ShouldExecute;

        private bool ShouldExecute 
        {
            set 
            {
                if (_ShouldExecute == value)
                    return;

                _ShouldExecute = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DebuggerStepThrough]
        public void Execute(object parameter) 
        {
            if ((parameter is T) || (parameter == null && TypeHelper.IsClass<T>()))
                Execute((T)parameter);
        }

        [DebuggerStepThrough]
        public async void Execute(T argument)
        {
            if (!_ShouldExecute)
                return;

            ShouldExecute = false;
            await _Execute(argument);
            ShouldExecute = true;
        }
    }
}
