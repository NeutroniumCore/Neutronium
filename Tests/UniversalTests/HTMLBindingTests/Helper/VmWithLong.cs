using Neutronium.Core.Test.Helper;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithLong : ViewModelTestBase
    {
        private long _LongValue;
        public long longValue
        {
            get => _LongValue;
            set => Set(ref _LongValue, value);
        }
    }
}
