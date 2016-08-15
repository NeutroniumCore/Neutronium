using IntegratedTest.Tests.WPF;
using Xunit;

namespace ChromiumFX.Window.Integrated.Tests
{
    [Collection("ChromiumFX Window Ko Integrated")]
    public class Test_DoubleNavigation_ChromiumFX : Test_DoubleNavigation
    {
        public Test_DoubleNavigation_ChromiumFX(ChromiumFXWindowKoTestEnvironment context) : base(context) 
        {         
        }
    }
}
