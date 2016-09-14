using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Neutronium.Core.Infra.VM
{
    public class BasicRelayCommand : ICommand
    {
        private readonly Action _Execute;
        private bool _Canexecute = true;

        public BasicRelayCommand(Action execute)
        {
            _Execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _Canexecute;
        }

        public bool Executable
        {
            get { return _Canexecute; }
            set
            {
                _Canexecute =value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            _Execute();
        }

        public event EventHandler CanExecuteChanged;
    }
}
