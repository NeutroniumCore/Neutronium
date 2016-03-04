using IntegratedTest.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests 
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_CefTask_Action_Awesomium : Test_CefTask_Action 
    {
        public Test_CefTask_Action_Awesomium(AwesomiumTestContext context) : base(context.GetWindowlessEnvironment())
        {
        }
    }
}
