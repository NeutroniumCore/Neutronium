using Mobx.ChromiumFx.Test.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Mobx.ChromiumFx.Test
{
    [Collection("Mobx Windowless Integrated")]
    public class BindingLifeCycle_Mobx_Cfx_Tests : BindingLifeCycleTests
    {
        public BindingLifeCycle_Mobx_Cfx_Tests(MobxVueContext testEnvironment, ITestOutputHelper output) :
            base(testEnvironment, output)
        {
        }
    }
}
