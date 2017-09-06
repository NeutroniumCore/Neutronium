namespace Neutronium.Core.Test.Helper
{
    public class ReadOnlyTestViewModel : ViewModelTestBase
    {
        private int _ReadOnly;
        public int ReadOnly
        {
            get { return _ReadOnly; }
            private set { Set(ref _ReadOnly, value); }
        }

        public void SetReadOnly(int newValue) => ReadOnly = newValue;
    }
}
