using IntegratedTest.Tests.WPF;
using MVVM.Awesomium.TestInfra;
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
