using Integrated.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.TestInfra;

namespace MVVM.Awesomium.Tests.Integrated.Window 
{
    public class Test_DoubleNavigation_Awesomium : Test_DoubleNavigation 
    {
        protected override WindowTestEnvironment GetEnvironment() 
        {
            return AwesomiumTestHelper.GetWindowEnvironment();
        }
    }
}
