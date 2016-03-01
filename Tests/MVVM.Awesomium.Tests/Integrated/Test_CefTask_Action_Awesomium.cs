using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;

namespace MVVM.Awesomium.Tests.Integrated 
{
    public class Test_CefTask_Action_Awesomium : Test_CefTask_Action 
    {
        public Test_CefTask_Action_Awesomium() : base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
