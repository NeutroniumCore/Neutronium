using Neutronium.Core.Test.Helper;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class IntViewModel : ViewModelTestBase
    {
        private int _Value;
        public int Value
        {
            get => _Value;
            set => Set(ref _Value, value);
        }
    }
}
