using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MVVM.HTML.Core.Infra.VM
{
    public class BasicRelayCommand : ICommand
    {
        readonly Action _execute;

        public BasicRelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            _execute();
        }

        event EventHandler ICommand.CanExecuteChanged { add { } remove { } }
    }
}
