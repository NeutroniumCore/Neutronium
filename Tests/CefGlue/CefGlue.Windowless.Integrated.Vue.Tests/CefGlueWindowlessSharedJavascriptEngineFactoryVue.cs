using CefGlue.TestInfra;
using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.IntegratedInfra;

namespace CefGlue.Windowless.Integrated.Vue.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryVue : CefGlueWindowlessSharedJavascriptEngineFactory
    {
        protected override FrameworkTestContext FrameworkTestContext => VueFrameworkTestContext.GetVueFrameworkTestContext();
    }
}
