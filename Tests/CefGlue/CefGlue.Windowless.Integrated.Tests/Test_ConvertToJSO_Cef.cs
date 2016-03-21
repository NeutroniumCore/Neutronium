using CefGlue.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Tests 
{
    [Collection("Cef Windowless Integrated")]
    public class Test_ConvertToJSO_Cef : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context, ITestOutputHelper output) :  
            base(context, output)
        {
        }
    }
}
