using Tests.CefGlue.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.WebBrowserEngineTests
{
    [Collection("CefGlue Context")]
    public class CefGlue_CSharpToGlueMapper_Tests : CSharpToGlueMapper_Tests 
    {
        public CefGlue_CSharpToGlueMapper_Tests(CefGlueContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
