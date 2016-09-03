using IntegratedTest.Tests.WPF;
using Ko.Navigation.ChromiumFx.Tests.Infra;
using Xunit;

namespace Ko.Navigation.ChromiumFx.Tests
{
    [Collection("Cfx Window Integrated")]
    public class HTMLViewControl_Awe_Cfx_Tests : HTMLViewControlTests
    {
        public HTMLViewControl_Awe_Cfx_Tests(CfxKoContext context) : base(context)
        {
        }
    }
}
