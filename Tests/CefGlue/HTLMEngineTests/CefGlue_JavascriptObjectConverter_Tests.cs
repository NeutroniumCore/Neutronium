using Tests.CefGlue.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.WebBrowserEngineTests
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
