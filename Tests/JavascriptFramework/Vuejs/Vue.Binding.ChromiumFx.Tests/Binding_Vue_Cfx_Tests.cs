using Vue.Binding.ChromiumFx.Tests.Infra;
using Xunit;
using Xunit.Abstractions;
using VueFramework.Test.IntegratedInfra;

namespace Vue.Binding.ChromiumFx.Tests
{
    [Collection("Cfx Vue Windowless Integrated")]
    public class Binding_Vue_Cfx_Tests : HTMLVueBindingTests
    {
        public Binding_Vue_Cfx_Tests(CfxVueContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
