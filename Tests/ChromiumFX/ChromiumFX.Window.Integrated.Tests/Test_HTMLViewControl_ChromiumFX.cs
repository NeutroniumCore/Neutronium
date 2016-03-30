using ChromiumFX.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests 
{
    [Collection("ChromiumFX Window Integrated")]
    public class Test_HTMLViewControl_ChromiumFX : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_ChromiumFX(ChromiumFXWindowTestEnvironment context): base(context) 
        {
        }
    }
}
