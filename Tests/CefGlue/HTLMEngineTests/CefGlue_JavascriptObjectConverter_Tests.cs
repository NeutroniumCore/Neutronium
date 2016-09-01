using Tests.CefGlue.HTLMEngineTests.Infra;
using Tests.Universal.HTLMEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.HTLMEngineTests
{
    [Collection("CefGlue Context")]
    public class CefGlue_JavascriptObjectConverter_Tests : JavascriptObjectConverter_Tests
    {
        public CefGlue_JavascriptObjectConverter_Tests(CefGlueContext testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }
    }
}
