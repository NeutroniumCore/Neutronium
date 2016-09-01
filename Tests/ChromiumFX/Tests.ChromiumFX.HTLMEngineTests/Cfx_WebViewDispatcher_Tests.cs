using Tests.ChromiumFX.HTLMEngineTests.Infra;
using Tests.Universal.HTLMEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.HTLMEngineTests
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
