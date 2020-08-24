using Tests.Awesomium.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Awesomium.WebBrowserEngineTests
{
    [Collection("Awesomium Context")]
    public class Awe_CSharpToGlueMapper_Tests : CSharpToGlueMapper_Tests 
    {
        public Awe_CSharpToGlueMapper_Tests(AwesomiumContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
