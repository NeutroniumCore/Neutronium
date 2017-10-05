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

        public bool ShouldExecute
        {
            get { return _ShouldExecute; }
            set { if (_ShouldExecute != value) { _ShouldExecute = value; FireCanExecuteChanged(); } }
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

        private void FireCanExecuteChanged() 
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
