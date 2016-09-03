using IntegratedTest.Tests.WPF;
using Ko.Navigation.ChromiumFx.Tests.Infra;
using Xunit;

namespace Ko.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Awe_Cfx_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Awe_Cfx_Tests(CfxKoContext context) : base(context)
        {
        }
    }
}
