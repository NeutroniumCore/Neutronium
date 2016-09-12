using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Neutronium.Example.ViewModel.Infra
{
   
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action execute)
            : this((_) => execute())
        {
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return true;
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

         event EventHandler ICommand.CanExecuteChanged { add{} remove{} }
    }


    public class RelayCommand<T> : ICommand where T:class
    {
        readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return true;
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            _execute(parameter as T);
        }

        event EventHandler ICommand.CanExecuteChanged { add { } remove { } }
    }

    public class ToogleRelayCommand : ICommand
    {
        readonly Action _execute;

        public ToogleRelayCommand(Action execute)
        {
            _execute = execute;
        }

        private bool _ShouldExecute=true;
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
            _execute();
        }

        private void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

         public event EventHandler CanExecuteChanged;
    }
}
