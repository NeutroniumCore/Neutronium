using ChromiumFX.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests 
{
    [Collection("ChromiumFX Window Integrated")]
    public class Test_DoubleNavigation_ChromiumFX : Test_DoubleNavigation
    {
        public Test_DoubleNavigation_ChromiumFX(ChromiumFXWindowKoTestEnvironment context) : base(context) 
        {         
        }
    }
}
