using Tests.ChromiumFX.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.WebBrowserEngineTests
{
    [Collection("ChromiumFX Context")]
    public class Cfx_JavascriptObjectConverter_Tests : JavascriptObjectConverter_Tests
    {
        public Cfx_JavascriptObjectConverter_Tests(ChromiumFXContext testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }
    }
}
