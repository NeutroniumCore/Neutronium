using Tests.ChromiumFX.HTMLEngineTests.Infra;
using Tests.Universal.HTMLEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.HTMLEngineTests
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
