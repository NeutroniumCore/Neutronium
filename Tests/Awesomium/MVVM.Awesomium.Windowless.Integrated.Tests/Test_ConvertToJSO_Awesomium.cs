using IntegratedTest.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Windowless.Integrated.Tests
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_ConvertToJSO_Awesomium  : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Awesomium(AwesomiumTestContext context) : base(context.GetWindowlessEnvironment())
        {
        }
    }
}
