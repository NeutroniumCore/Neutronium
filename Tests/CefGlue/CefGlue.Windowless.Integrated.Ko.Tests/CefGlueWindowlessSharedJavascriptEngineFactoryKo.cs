using CefGlue.TestInfra;
using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryKo : IWindowLessHTMLEngineProvider 
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();

        public FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;
        public IBasicWindowLessHTMLEngineProvider WindowBuilder { get; } = new CefGlueWindowlessSharedJavascriptEngineFactory(KoTestContext.HtmlProvider);
    }
}
