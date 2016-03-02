using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Cef.Glue.Tests.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_HTMLViewControl_Cef : Test_HTMLViewControl
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return CefTestHelper.GetWindowEnvironment();
        }
    }
}
