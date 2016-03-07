using CefGlue.TestInfra;
using IntegratedTest.WPF;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_DoubleNavigation_Cef : Test_DoubleNavigation, IClassFixture<CefWindowTestEnvironment> 
    {
        public Test_DoubleNavigation_Cef(CefWindowTestEnvironment context) : base(context) 
        {

        }
    }
}
