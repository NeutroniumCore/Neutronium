using Neutronium.Core.Test.Helper;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithTwoStrings : ViewModelTestBase
    {
        public VmWithTwoStrings(string first, string second)
        {
            _String1 = first;
            _String2 = second;
        }

        private string _String1;
        public string String1
        {
            get => _String1;
            set => Set(ref _String1, value);
        }

        private string _String2;
        public string String2
        {
            get => _String2;
            set => Set(ref _String2, value);
        }
    }
}
