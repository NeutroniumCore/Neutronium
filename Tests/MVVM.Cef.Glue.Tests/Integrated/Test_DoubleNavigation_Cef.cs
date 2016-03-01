using Integrated.WPF;
using IntegratedTest;
using IntegratedTest.WPF.Infra;
using MVVM.Cef.Glue.Tests.Infra;

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
