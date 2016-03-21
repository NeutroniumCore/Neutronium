using CefGlue.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Integrated")]
    public class Test_HTMLBinding_Cef : Test_HTMLBinding
    {
        public Test_HTMLBinding_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context, ITestOutputHelper output) : 
            base(context, output)
        {
        }
    }
}

