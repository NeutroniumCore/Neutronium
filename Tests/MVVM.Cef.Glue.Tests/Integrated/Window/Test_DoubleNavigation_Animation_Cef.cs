using CefGlue.TestInfra;
using IntegratedTest.WPF;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    [Collection("Cef Window Integrated")]
    public class Test_DoubleNavigation_Animation_Cef : Test_DoubleNavigation_Animation
    {
        public Test_DoubleNavigation_Animation_Cef(CefWindowTestEnvironment context): base(context) 
        {
        }  
    }
}
