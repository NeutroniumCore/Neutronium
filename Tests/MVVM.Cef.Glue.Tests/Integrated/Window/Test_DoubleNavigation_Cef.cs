using CefGlue.TestInfra;
using Integrated.WPF;
using IntegratedTest.WPF.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_DoubleNavigation_Cef : Test_DoubleNavigation
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return CefTestHelper.GetWindowEnvironment();
        }
    }
}
