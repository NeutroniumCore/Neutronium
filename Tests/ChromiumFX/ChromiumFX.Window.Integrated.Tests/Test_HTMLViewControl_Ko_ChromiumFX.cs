using IntegratedTest.Tests.WPF;
using Xunit;

namespace ChromiumFX.Window.Integrated.Tests
{
    [Collection("ChromiumFX Window Ko Integrated")]
    public class Test_HTMLViewControl_Ko_ChromiumFX : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Ko_ChromiumFX(ChromiumFXWindowKoTestEnvironment context): base(context) 
        {
        }
    }
}
