using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Tests.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_ConvertToJSO_Cef : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Cef() : base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
