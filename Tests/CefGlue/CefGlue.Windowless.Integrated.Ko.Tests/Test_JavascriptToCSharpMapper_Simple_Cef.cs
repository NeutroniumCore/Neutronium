using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    [Collection("Cef Windowless Ko Integrated")]
    public class Test_JavascriptToCSharpMapper_Simple_Cef : Test_JavascriptToCSharpMapper_Simple
    {
        public Test_JavascriptToCSharpMapper_Simple_Cef(CefGlueWindowlessSharedJavascriptEngineFactoryKo context, ITestOutputHelper output) :
            base(context, output)
        {
        }
    }
}
