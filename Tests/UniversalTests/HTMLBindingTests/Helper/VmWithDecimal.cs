using Neutronium.Core.Test.Helper;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithDecimal : ViewModelTestBase
    {
        private decimal _DecimalValue;
        public decimal decimalValue
        {
            get => _DecimalValue;
            set => Set(ref _DecimalValue, value);
        }
    }
}
