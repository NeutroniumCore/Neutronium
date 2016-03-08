using CefGlue.TestInfra;
using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    [CollectionDefinition("Cef Window Integrated")]
    public class Test_HTMLViewControl_Cef : Test_HTMLViewControl
    {
        public Test_HTMLViewControl_Cef(CefWindowTestEnvironment context, WpfThread wpfThread)
            : base(context, wpfThread) 
        {
        }
    }
}
