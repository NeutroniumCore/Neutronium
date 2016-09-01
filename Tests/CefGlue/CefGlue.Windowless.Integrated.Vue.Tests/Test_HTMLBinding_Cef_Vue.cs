using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace CefGlue.Windowless.Integrated.Vue.Tests
{
    [Collection("Cef Windowless Vue Integrated")]
    public class Test_HTMLBinding_Cef_Vue : Test_HTMLBinding
    {
        public Test_HTMLBinding_Cef_Vue(CefGlueWindowlessSharedJavascriptEngineFactoryVue context, ITestOutputHelper output) : 
            base(context, output)
        {
        }
    }
}

