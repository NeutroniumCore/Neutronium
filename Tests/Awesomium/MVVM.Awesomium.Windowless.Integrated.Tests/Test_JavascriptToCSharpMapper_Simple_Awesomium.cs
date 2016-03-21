using IntegratedTest.Tests.Windowless;
using MVVM.Awesomium.TestInfra;
using Xunit;
using Xunit.Abstractions;

namespace MVVM.Awesomium.Tests.Integrated
{
    [Collection("Awesomium Windowless Integrated")]
    public class Test_JavascriptToCSharpMapper_Simple_Awesomium : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Awesomium(AwesomiumTestContext context, ITestOutputHelper output) 
            : base(context, output)
        {
        }
    }
}
