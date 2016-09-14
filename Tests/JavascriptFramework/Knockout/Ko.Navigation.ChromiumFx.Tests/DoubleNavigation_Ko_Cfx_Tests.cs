using Ko.Navigation.ChromiumFx.Tests.Infra;
using Tests.Universal.NavigationTests;
using Xunit;

namespace Ko.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Ko_Cfx_Tests : DoubleNavigationTests
    {
        public DoubleNavigation_Ko_Cfx_Tests(CfxKoContext context) : base(context)
        {
        }
    }
}
