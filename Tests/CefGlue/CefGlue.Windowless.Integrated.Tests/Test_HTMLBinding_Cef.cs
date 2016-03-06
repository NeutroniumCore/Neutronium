using CefGlue.TestInfra;
using IntegratedTest.Windowless;
using Xunit;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Integrated")]
    public class Test_HTMLBinding_Cef : Test_HTMLBinding,  IClassFixture<CefGlueWindowlessSharedJavascriptEngineFactory> 
    {
        public Test_HTMLBinding_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context) : base(context)
        {
        }
    }
}

