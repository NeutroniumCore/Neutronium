using CefGlue.TestInfra;
using IntegratedTest.WPF;
using IntegratedTest.WPF.Infra;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    [CollectionDefinition("Cef Window Integrated")]
    public class Test_DoubleNavigation_Cef : Test_DoubleNavigation 
    {
        public Test_DoubleNavigation_Cef(CefWindowTestEnvironment context, WpfThread wpfThread) : base(context, wpfThread) 
        {
        }
    }
}
