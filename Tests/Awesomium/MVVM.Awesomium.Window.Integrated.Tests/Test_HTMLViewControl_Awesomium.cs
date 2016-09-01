using IntegratedTest.Tests.WPF;
using Tests.Awesomium.Infra;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests 
{
    [Collection("Awesomium Window Integrated")]
    public class Test_HTMLViewControl_Awesomium : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Awesomium(AwesomiumWindowTestEnvironment context): base(context) 
        {
        }
    }
}
