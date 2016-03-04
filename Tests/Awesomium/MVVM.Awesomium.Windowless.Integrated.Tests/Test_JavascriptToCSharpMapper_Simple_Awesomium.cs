using IntegratedTest.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;

namespace MVVM.Awesomium.Tests.Integrated
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_JavascriptToCSharpMapper_Simple_Awesomium : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Awesomium(AwesomiumTestContext context) 
            : base(context.GetWindowlessEnvironment())
        {
        }
    }
}
