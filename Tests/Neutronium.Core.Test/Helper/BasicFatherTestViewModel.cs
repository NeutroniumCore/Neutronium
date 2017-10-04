using System.Windows.Input;
using Neutronium.MVVMComponents.Relay;

namespace Neutronium.Core.Test.Helper
{
    public class BasicFatherTestViewModel : ViewModelTestBase
    {
        private BasicTestViewNodel _Child;
        public BasicTestViewNodel Child
        {
            get { return _Child; }
            set { Set(ref _Child, value); }
        }

        public ICommand Command { get; }

        public BasicTestViewNodel LastCallElement { get; private set; }

        public int CallCount { get; private set; } = 0;

        public BasicFatherTestViewModel()
        {
            Command = new RelayCommand<BasicTestViewNodel>(child =>
            {
                CallCount++;
                LastCallElement = child;
            });
        }
    }
}
