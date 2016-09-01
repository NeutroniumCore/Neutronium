using Tests.ChromiumFX.HTLMEngineTests.Infra;
using Tests.Universal.HTLMEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.HTLMEngineTests
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
