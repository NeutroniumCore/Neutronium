using System.Windows.Input;
using Neutronium.MVVMComponents;

namespace Neutronium.Core.Test.Helper
{
    public class FakeTestViewModel : ViewModelTestBase
    {
        private ICommand _Command;
        public ICommand Command
        {
            get { return _Command; }
            set { Set(ref _Command, value); }
        }

        private ICommand<string> _CommandGeneric;
        public ICommand<string> CommandGeneric
        {
            get { return _CommandGeneric; }
            set { Set(ref _CommandGeneric, value); }
        }

        private ICommand<FakeTestViewModel> _AutoCommand;
        public ICommand<FakeTestViewModel> AutoCommand
        {
            get { return _AutoCommand; }
            set { Set(ref _AutoCommand, value); }
        }

        private ICommand<int> _CommandGenericInt;
        public ICommand<int> CommandGenericInt
        {
            get { return _CommandGenericInt; }
            set { Set(ref _CommandGenericInt, value); }
        }

        private ICommandWithoutParameter _CommandWithoutParameters;
        public ICommandWithoutParameter CommandWithoutParameters
        {
            get { return _CommandWithoutParameters; }
            set { Set(ref _CommandWithoutParameters, value); }
        }

        public string Name => "NameTest";

        public string UselessName { set { } }

        public void InconsistentEventEmit()
        {
            this.OnPropertyChanged("NonProperty");
        }
    }
}
