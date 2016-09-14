using Tests.CefGlue.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.WebBrowserEngineTests
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
