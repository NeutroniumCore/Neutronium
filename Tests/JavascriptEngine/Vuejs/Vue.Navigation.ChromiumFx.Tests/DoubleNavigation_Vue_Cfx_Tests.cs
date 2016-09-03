using IntegratedTest.Tests.WPF;
using Vue.Navigation.ChromiumFx.Tests.Infra;
using Xunit;

namespace Vue.Navigation.ChromiumFx.Tests.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Vue_Cfx_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Vue_Cfx_Tests(CfxVueContext context) : base(context)
        {
        }
    }
}
