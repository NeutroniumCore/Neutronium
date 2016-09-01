using Tests.CefGlue.HTLMEngineTests.Infra;
using Tests.Universal.HTLMEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.CefGlue.HTLMEngineTests
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
