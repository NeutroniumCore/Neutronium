using CefGlue.TestInfra;
using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;

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
