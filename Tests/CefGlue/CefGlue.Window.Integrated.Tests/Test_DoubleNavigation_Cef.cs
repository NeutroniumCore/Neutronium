using CefGlue.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace CefGlue.Window.Integrated.Tests
{
    [Collection("Cef Window Integrated")]
    public class Test_DoubleNavigation_Cef : Test_DoubleNavigation 
    {
        public Test_DoubleNavigation_Cef(CefWindowTestEnvironment context) : base(context) 
        {
        }
    }
}
