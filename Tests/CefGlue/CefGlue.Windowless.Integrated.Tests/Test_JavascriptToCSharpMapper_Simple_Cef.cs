using CefGlue.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Integrated")]
    public class Test_JavascriptToCSharpMapper_Simple_Cef : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context, ITestOutputHelper output) :
            base(context, output)
        {
        }
    }
}
