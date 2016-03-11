using CefGlue.TestInfra;
using IntegratedTest.Tests.WPF;
using Xunit;

namespace CefGlue.Window.Integrated.Tests
{
    [Collection("Cef Window Integrated")]
    public class Test_HTMLViewControl_Cef : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Cef(CefWindowTestEnvironment context): base(context) 
        {
        }
    }
}
