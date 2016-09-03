using IntegratedTest.Tests.WPF;
using Ko.Navigation.ChromiumFx.Tests.Infra;
using Xunit;

namespace Ko.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Cfx_Ko_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Cfx_Ko_Tests(CfxKoContext context) : base(context)
        {
        }
    }
}
