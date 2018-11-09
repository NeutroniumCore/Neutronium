namespace Neutronium.Core.Test.Helper
{
    public class BasicTestViewModel : ViewModelTestBase
    {
        private BasicTestViewModel _Child;
        public BasicTestViewModel Child
        {
            get => _Child;
            set => Set(ref _Child, value);
        }

        private int _Value = -1;
        public int Value
        {
            get => _Value;
            set => Set(ref _Value, value);
        }
    }
}
