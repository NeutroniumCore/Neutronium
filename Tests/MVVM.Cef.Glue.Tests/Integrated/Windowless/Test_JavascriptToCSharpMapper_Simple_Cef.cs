using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Tests.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_JavascriptToCSharpMapper_Simple_Cef : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Cef(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
