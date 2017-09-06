namespace Neutronium.Core.Test.Helper
{
    public class SimpleFatherTestViewModel : ViewModelTestBase
    {
        private SimpleReadOnlyTestViewModel _SimpleReadOnlyTestViewModel;
        public SimpleReadOnlyTestViewModel Other
        {
            get { return _SimpleReadOnlyTestViewModel; }
            set { Set(ref _SimpleReadOnlyTestViewModel, value); }
        }
    }
}
