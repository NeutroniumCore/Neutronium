using CefGlue.TestInfra;
using Mobx.Test.IntegratedInfra;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;

namespace Mobx.CefGlue.Test.Infra
{
    public class CefGlueMobxContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext MobxTestContext { get; } = MobxFrameworkTestContext.GetMobxFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = MobxTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new CefGlueWindowlessSharedJavascriptEngineFactory(MobxTestContext.HtmlProvider);
    }
}
