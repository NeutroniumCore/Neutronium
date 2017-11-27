using System;

namespace Neutronium.Core.Test.Helper
{
    public class FactoryFatherTestViewModel : ViewModelTestBase
    {
        private BasicTestViewModel _Child;
        public BasicTestViewModel Child
        {
            get { return _Child; }
            set { Set(ref _Child, value); }
        }

        public FakeFactory<int, BasicTestViewModel> Factory { get; }

        public FactoryFatherTestViewModel(Func<int, BasicTestViewModel> fact)
        {
            Factory = new FakeFactory<int, BasicTestViewModel>(fact);
        }
    }
}
