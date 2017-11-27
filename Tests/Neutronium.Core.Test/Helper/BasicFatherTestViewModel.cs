using System.Windows.Input;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Core.Test.Helper
{
    public class BasicFatherTestViewModel : ViewModelTestBase
    {
        private BasicTestViewModel _Child;
        public BasicTestViewModel Child
        {
            get { return _Child; }
            set { Set(ref _Child, value); }
        }

        public ICommand Command { get; }

        public BasicTestViewModel LastCallElement { get; private set; }

        public int CallCount { get; private set; } = 0;

        public BasicFatherTestViewModel()
        {
            Command = new RelaySimpleCommand<BasicTestViewModel>(child =>
            {
                CallCount++;
                LastCallElement = child;
            });
        }
    }
}
