using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Tests.Infra;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_CefTask_Action_Cef : Test_CefTask_Action
    {
        public Test_CefTask_Action_Cef(): base(CefTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
