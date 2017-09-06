using System.Windows.Input;

namespace Neutronium.Core.Test.Helper
{
    public class CommandsTestViewModel : ViewModelTestBase
    {
        private ICommand[] _Commands;
        public ICommand[] Commands
        {
            get { return _Commands; }
            set { Set(ref _Commands, value); }
        }
    }
}
