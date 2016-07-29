using ChromiumFX.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace MVVM.Awesomium.Window.Integrated.Tests 
{
    [Collection("ChromiumFX Window Ko Integrated")]
    public class Test_DoubleNavigation_Animation_ChromiumFX : Test_DoubleNavigation_Animation
    {
        public Test_DoubleNavigation_Animation_ChromiumFX(ChromiumFXWindowKoTestEnvironment context) : base(context) 
        {
        }
    }
}
