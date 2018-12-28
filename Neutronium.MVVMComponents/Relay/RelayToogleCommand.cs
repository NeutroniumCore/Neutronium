using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayToogleCommand : ICommand, ICommandWithoutParameter
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action _Execute;
        private bool _ShouldExecute = true;

        public RelayToogleCommand(Action execute)
        {
            _Execute = execute;
        }

        public RelayToogleCommand(Action execute, bool shouldExecute)
        {
            _Execute = execute;
            _ShouldExecute = shouldExecute;
        }

        public bool ShouldExecute
        {
            get => _ShouldExecute;
            set 
            {
                if (_ShouldExecute == value)
                    return;

                _ShouldExecute = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        bool ICommandWithoutParameter.CanExecute => _ShouldExecute;

        public bool CanExecute(object parameter) => _ShouldExecute;

        [DebuggerStepThrough]
        public void Execute(object parameter) => Execute();

        [DebuggerStepThrough]
        public void Execute() 
        {
            if (_ShouldExecute)
                _Execute();
        }
    }
}
