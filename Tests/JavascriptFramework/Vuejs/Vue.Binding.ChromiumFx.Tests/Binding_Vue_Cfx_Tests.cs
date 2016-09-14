using Vue.Binding.ChromiumFx.Tests.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Binding.ChromiumFx.Tests
{
    [Collection("Cfx Vue Windowless Integrated")]
    public class Binding_Vue_Cfx_Tests : HTMLBindingTests
    {
        public Binding_Vue_Cfx_Tests(CfxVueContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
