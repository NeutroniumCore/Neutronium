using CefGlue.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Tests
{
    [Collection("Cef Windowless Ko Integrated")]
    public class Test_HTMLBinding_Cef_Ko : Test_HTMLBinding
    {
        public Test_HTMLBinding_Cef_Ko(CefGlueWindowlessSharedJavascriptEngineFactoryKo context, ITestOutputHelper output) : 
            base(context, output)
        {
        }
    }
}

