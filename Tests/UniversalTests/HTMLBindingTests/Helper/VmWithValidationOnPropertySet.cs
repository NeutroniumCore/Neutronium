using Neutronium.Core.Test.Helper;

namespace Tests.Universal.HTMLBindingTests.Helper
{
    public class VmWithValidationOnPropertySet : ViewModelTestBase
    {
        private int _MagicNumber;
        public int MagicNumber
        {
            get => _MagicNumber;
            set
            {
                if (value == 9)
                {
                    value = 42;
                }
                Set(ref _MagicNumber, value);
            }
        }
    }
}
