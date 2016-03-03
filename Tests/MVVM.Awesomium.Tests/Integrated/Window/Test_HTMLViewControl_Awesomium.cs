using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using MVVM.Awesomium.Tests.Infra;

namespace MVVM.Awesomium.Tests.Integrated.Window {
    public class Test_HTMLViewControl_Awesomium : Test_HTMLViewControl
    {
        protected override WindowTestEnvironment GetEnvironment()
        {
            return AwesomiumTestHelper.GetWindowEnvironment();
        }
    }
}
