using Tests.ChromiumFX.HTLMEngineTests.Infra;
using Tests.Universal.HTLMEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.HTLMEngineTests
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
