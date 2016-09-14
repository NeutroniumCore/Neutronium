using Tests.ChromiumFX.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.WebBrowserEngineTests 
{
    [Collection("ChromiumFX Context")]
    public class Cfx_CSharpToJavascriptConverter_Tests : CSharpToJavascriptConverter_Tests 
    {
        public Cfx_CSharpToJavascriptConverter_Tests(ChromiumFXContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
