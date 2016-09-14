using Tests.Universal.NavigationTests;
using Vue.Navigation.ChromiumFx.Tests.Infra;
using Xunit;

namespace Vue.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class HTMLViewControl_Vue_Cfx_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Vue_Cfx_Tests(CfxVueContext context) : base(context)
        {
        }
    }
}
