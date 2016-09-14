using Tests.CefGlue.HTMLEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.HTMLEngineTests
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
