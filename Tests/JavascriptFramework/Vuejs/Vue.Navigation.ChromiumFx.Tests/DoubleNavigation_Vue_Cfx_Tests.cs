using Tests.Universal.NavigationTests;
using Vue.Navigation.ChromiumFx.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Navigation.ChromiumFx.Tests.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Vue_Cfx_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Vue_Cfx_Tests(CfxVueContext context, ITestOutputHelper testOutputHelper) : base(context, testOutputHelper)
        {
        }
    }
}
