using Tests.CefGlue.HTLMEngineTests.Infra;
using Tests.Universal.HTLMEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.HTLMEngineTests
{
    [Collection("CefGlue Context")]
    public class CefGlue_WebViewDispatcher_Tests : WebViewDispatcher_Tests
    {
        public CefGlue_WebViewDispatcher_Tests(CefGlueContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
