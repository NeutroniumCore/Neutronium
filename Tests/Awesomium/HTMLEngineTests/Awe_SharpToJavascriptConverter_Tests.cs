using Tests.Awesomium.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Awesomium.WebBrowserEngineTests
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
