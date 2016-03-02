using IntegratedTest.Windowless;
using MVVM.Awesomium.Tests.Infra;

namespace MVVM.Awesomium.Tests.Integrated
{
    public class Test_ConvertToJSO_Awesomium  : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Awesomium(): base(AwesomiumTestHelper.GetWindowlessEnvironment())
        {
        }
    }
}
