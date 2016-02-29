using IntegratedTest;
using IntegratedTest.WPF;
using MVVM.Cef.Glue.Tests.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_DoubleNavigation_Animation_Cef : Test_DoubleNavigation_Animation
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return CefTestHelper.GetWindowEnvironment();
        }    
    }
}
