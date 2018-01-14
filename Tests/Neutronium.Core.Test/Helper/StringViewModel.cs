namespace Neutronium.Core.Test.Helper
{
    public class BasicTestViewModel : ViewModelTestBase
    {
        private BasicTestViewModel _Child;
        public BasicTestViewModel Child
        {
            get { return _Child; }
            set { Set(ref _Child, value); }
        }

        private int _Value = -1;
        public int Value
        {
            get { return _Value; }
            set { Set(ref _Value, value); }
        }
    }
}
