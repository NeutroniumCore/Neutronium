using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.TestInfra;

namespace MVVM.Awesomium.Tests.Integrated.Window 
{
    public class Test_DoubleNavigation_Animation_Awesomium : Test_DoubleNavigation_Animation
    {
        protected override WindowTestEnvironment GetEnvironment() 
        {
            return AwesomiumTestHelper.GetWindowEnvironment();
        }
    }
}
