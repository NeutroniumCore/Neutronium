using Tests.Awesomium.HTMLEngineTests.Infra;
using Tests.Universal.WebBrowserEnginesTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Awesomium.HTMLEngineTests
{
    [Collection("Awesomium Context")]
    public class Awe_SharpToJavascriptConverter_Tests : CSharpToJavascriptConverter_Tests 
    {
        public Awe_SharpToJavascriptConverter_Tests(AwesomiumContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
