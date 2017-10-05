using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neutronium.MVVMComponents.Relay
{
    public class RelayTaskCommand : ICommand, ICommandWithoutParameter
    {
        public event EventHandler CanExecuteChanged;
        private readonly Func<Task> _Execute;
        private bool _ShouldExecute = true;

        public RelayTaskCommand(Func<Task> execute)
        {
            _Execute = execute;
        }

        bool ICommandWithoutParameter.CanExecute => _ShouldExecute;

        public bool CanExecute(object parameter) => _ShouldExecute;

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
        public void Execute(object parameter) => Execute();

        [DebuggerStepThrough]
        public async void Execute() 
        {
            if (!_ShouldExecute)
                return;

            ShouldExecute = false;
            await _Execute();
            ShouldExecute = true;
        }
    }
}
