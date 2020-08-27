namespace Neutronium.Core.Test.Helper
{
    public class BasicTestTwoChildrenViewModel : ViewModelTestBase
    {
        private BasicTestViewModel _Child1;
        public BasicTestViewModel Child1
        {
            get => _Child1;
            set => Set(ref _Child1, value);
        }

        private BasicTestViewModel _Child2;
        public BasicTestViewModel Child2
        {
            get => _Child2;
            set => Set(ref _Child2, value);
        }

        private int _Value = -1;
        public int Value
        {
            get => _Value;
            set => Set(ref _Value, value);
        }
    }
}
