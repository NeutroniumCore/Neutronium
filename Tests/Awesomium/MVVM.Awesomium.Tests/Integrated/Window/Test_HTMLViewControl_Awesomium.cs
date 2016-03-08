using IntegratedTest.WPF;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated.Window 
{
    [Collection("Awesomium Window Integrated")]
    public class Test_HTMLViewControl_Awesomium : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Awesomium(AwesomiumWindowTestEnvironment context): base(context) 
        {
        }
    }
}
