using CefGlue.TestInfra;
using IntegratedTest.WPF;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    [Collection("Cef Window Integrated")]
    public class Test_DoubleNavigation_Cef : Test_DoubleNavigation 
    {
        public Test_DoubleNavigation_Cef(CefWindowTestEnvironment context) : base(context) 
        {
        }
    }
}
