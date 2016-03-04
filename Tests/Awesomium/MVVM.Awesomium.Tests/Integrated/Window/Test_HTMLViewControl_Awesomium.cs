using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.TestInfra;

namespace MVVM.Awesomium.Tests.Integrated.Window {
    public class Test_HTMLViewControl_Awesomium : Test_HTMLViewControl
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return AwesomiumTestHelper.GetWindowEnvironment();
        }
    }
}
