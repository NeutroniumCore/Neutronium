using System;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayToogleCommand<T> : ICommand, ICommand<T>
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<T> _Execute;

        public RelayToogleCommand(Action<T> execute)
        {
            _Execute = execute;
        }

        private bool _ShouldExecute = true;
        public bool ShouldExecute
        {
            get { return _ShouldExecute; }
            set 
            {
                if (_ShouldExecute == value)
                    return;

                _ShouldExecute = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter) => _ShouldExecute;

        public void Execute(object parameter) 
        {
            if ((parameter is T) || (parameter == null && (typeof(T).IsClass)))
                Execute((T)parameter);
        }

        public void Execute(T argument)
        {
            if (_ShouldExecute)
                _Execute(argument);
        }

        public bool CanExecute(T parameter) => _ShouldExecute;
    }
}
