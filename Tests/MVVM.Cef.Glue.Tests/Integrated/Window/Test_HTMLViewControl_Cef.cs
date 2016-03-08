using CefGlue.TestInfra;
using IntegratedTest.WPF;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    [Collection("Cef Window Integrated")]
    public class Test_HTMLViewControl_Cef : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Cef(CefWindowTestEnvironment context): base(context) 
        {
        }
    }
}
