using Tests.ChromiumFX.HTMLEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.HTMLEngineTests
{
    [Collection("ChromiumFX Context")]
    public class Cfx_WebViewDispatcher_Tests : WebViewDispatcher_Tests
    {
        public Cfx_WebViewDispatcher_Tests(ChromiumFXContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
