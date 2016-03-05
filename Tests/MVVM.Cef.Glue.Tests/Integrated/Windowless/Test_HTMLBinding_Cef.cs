using IntegratedTest.Windowless;
using MVVM.Cef.Glue.Tests.Infra;
using Xunit;

namespace MVVM.Cef.Glue.Tests.Integrated
{
    public class Test_HTMLBinding_Cef : Test_HTMLBinding,  IClassFixture<CefGlueWindowlessSharedJavascriptEngineFactory> 
    {
        public Test_HTMLBinding_Cef(CefGlueWindowlessSharedJavascriptEngineFactory context) : base(context.GetWindowlessEnvironment())
        {
        }
    }
}

