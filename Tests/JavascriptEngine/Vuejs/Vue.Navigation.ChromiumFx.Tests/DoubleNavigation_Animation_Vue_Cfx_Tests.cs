using IntegratedTest.Tests.WPF;
using Vue.Navigation.ChromiumFx.Tests.Infra;
using Xunit;

namespace Vue.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class DoubleNavigation_Animation_Vue_Cfx_Tests : DoubleNavigation_AnimationTests
    {
        public DoubleNavigation_Animation_Vue_Cfx_Tests(CfxVueContext context) : base(context)
        {
        }
    }
}
