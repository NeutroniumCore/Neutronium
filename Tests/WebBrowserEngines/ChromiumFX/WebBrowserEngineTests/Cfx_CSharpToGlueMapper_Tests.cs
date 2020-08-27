using Tests.ChromiumFX.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.WebBrowserEngineTests 
{
    [Collection("ChromiumFX Context")]
    public class Cfx_CSharpToGlueMapper_Tests : CSharpToGlueMapper_Tests 
    {
        public Cfx_CSharpToGlueMapper_Tests(ChromiumFXContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
