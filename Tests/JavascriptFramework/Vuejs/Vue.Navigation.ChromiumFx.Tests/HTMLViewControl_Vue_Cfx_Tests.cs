using Tests.Universal.NavigationTests;
using Vue.Navigation.ChromiumFx.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class HTMLViewControl_Vue_Cfx_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Vue_Cfx_Tests(CfxVueContext context, ITestOutputHelper testOutputHelper) 
            : base(context, testOutputHelper)
        {
        }
    }
}
