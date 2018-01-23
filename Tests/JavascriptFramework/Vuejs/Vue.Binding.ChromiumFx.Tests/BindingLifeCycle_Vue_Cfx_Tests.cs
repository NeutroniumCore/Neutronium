using Tests.Universal.HTMLBindingTests;
using Vue.Binding.ChromiumFx.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Binding.ChromiumFx.Tests
{
    [Collection("Cfx Vue Windowless Integrated")]
    public class BindingLifeCycle_Vue_Cfx_Tests : BindingLifeCycleTests
    {
        public BindingLifeCycle_Vue_Cfx_Tests(CfxVueContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
