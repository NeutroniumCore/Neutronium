using CefGlue.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Integrated")]
    public class Test_WebView_Dispatcher_Cef : Test_WebView_Dispatcher
    {
        public Test_WebView_Dispatcher_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context, ITestOutputHelper output) : 
            base(context, output)
        {
        }
    }
}
