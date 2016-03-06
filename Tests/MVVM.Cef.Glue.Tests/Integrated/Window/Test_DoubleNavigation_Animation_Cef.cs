using CefGlue.TestInfra;
using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;

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
