using IntegratedTest.Tests.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;
using Xunit.Abstractions;

namespace MVVM.Awesomium.Windowless.Integrated.Tests
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_ConvertToJSO_Awesomium  : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Awesomium(AwesomiumTestContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
