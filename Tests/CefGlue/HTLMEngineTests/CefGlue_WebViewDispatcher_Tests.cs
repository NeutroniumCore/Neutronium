using Tests.CefGlue.HTMLEngineTests.Infra;
using Tests.Universal.WebBrowserEnginesTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.HTMLEngineTests
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
