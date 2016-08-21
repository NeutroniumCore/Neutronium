using CefGlue.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace CefGlue.Window.Integrated.Tests
{
    [Collection("Cef Window Integrated")]
    public class Test_DoubleNavigation_Animation_Cef : Test_DoubleNavigation_Animation
    {
        public Test_DoubleNavigation_Animation_Cef(CefWindowTestEnvironmentKo context): base(context) 
        {
        }  
    }
}
