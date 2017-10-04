using System;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    public class ToogleRelayCommand : ICommand
    {
        private readonly Action _Execute;

        public ToogleRelayCommand(Action execute)
        {
            _Execute = execute;
        }

        private bool _ShouldExecute = true;
        public bool ShouldExecute
        {
            get { return _ShouldExecute; }
            set { if (_ShouldExecute != value) { _ShouldExecute = value; FireCanExecuteChanged(); } }
        }

        public bool CanExecute(object parameter)
        {
            return _ShouldExecute;
        }

        public void Execute(object parameter)
        {
            _Execute();
        }

        private void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
