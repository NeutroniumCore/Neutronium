using CefGlue.TestInfra;
using KnockoutUIFramework.Test.IntegratedInfra;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryKo : WindowLessHTMLEngineProvider 
    {
        private static FrameworkTestContext KoTestContext { get; } = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
        public override FrameworkTestContext FrameworkTestContext { get; } = KoTestContext;
        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new CefGlueWindowlessSharedJavascriptEngineFactory(KoTestContext.HtmlProvider);
    }
}
