using Neutronium.MVVMComponents;

namespace Neutronium.Core.Test.Helper 
{
    public class SimpleCommandTestViewModel : ViewModelTestBase
    {
        public ISimpleCommand SimpleCommandNoArgument { get; set;  }

        public ISimpleCommand<SimpleCommandTestViewModel> SimpleCommandObjectArgument { get; set; }

        public ISimpleCommand<decimal> SimpleCommandDecimalArgument { get; set; }
    }
}
