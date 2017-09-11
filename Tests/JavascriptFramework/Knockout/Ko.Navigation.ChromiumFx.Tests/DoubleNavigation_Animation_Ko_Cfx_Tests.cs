using Ko.Navigation.ChromiumFx.Tests.Infra;
using Tests.Universal.NavigationTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Animation_Ko_Cfx_Tests : DoubleNavigation_AnimationTests
    {
        public DoubleNavigation_Animation_Ko_Cfx_Tests(CfxKoContext context, ITestOutputHelper testOutputHelper)
            : base(context, testOutputHelper)
        {
        }
    }
}
