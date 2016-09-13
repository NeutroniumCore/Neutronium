using Tests.CefGlue.HTMLEngineTests.Infra;
using Tests.Universal.WebBrowserEnginesTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.HTMLEngineTests
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
