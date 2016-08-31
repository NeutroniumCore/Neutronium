using CefGlue.TestInfra;
using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryKo : CefGlueWindowlessSharedJavascriptEngineFactory
    {
        protected override FrameworkTestContext FrameworkTestContext => KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
    }
}
