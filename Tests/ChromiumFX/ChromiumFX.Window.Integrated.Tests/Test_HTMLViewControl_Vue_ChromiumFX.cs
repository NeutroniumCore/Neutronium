using ChromiumFX.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests 
{
    [Collection("ChromiumFX Window Vue Integrated")]
    public class Test_HTMLViewControl_Vue_ChromiumFX : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Vue_ChromiumFX(ChromiumFXWindowVueTestEnvironment context): base(context) 
        {
        }
    }
}
