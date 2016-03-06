using CefGlue.TestInfra;
using IntegratedTest.Windowless;
using Xunit;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Integrated")]
    public class Test_WebView_Dispatcher_Cef : Test_WebView_Dispatcher
    {
        public Test_WebView_Dispatcher_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context) : 
            base(context)
        {
        }
    }
}
