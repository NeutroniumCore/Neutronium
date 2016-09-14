using Tests.CefGlue.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.WebBrowserEngineTests
{
    [Collection("CefGlue Context")]
    public class CefGlue_SharpToJavascriptConverter_Tests : CSharpToJavascriptConverter_Tests 
    {
        public CefGlue_SharpToJavascriptConverter_Tests(CefGlueContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
