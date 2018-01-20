using Mobx.Test.IntegratedInfra;
using Tests.ChromiumFX.Infra;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;

namespace Mobx.ChromiumFx.Test.Infra
{
    public class MobxVueContext : WindowLessHTMLEngineProvider
    {
        private static FrameworkTestContext MobxTestContext { get; } = MobxFrameworkTestContext.GetMobxFrameworkTestContext();

        public override FrameworkTestContext FrameworkTestContext { get; } = MobxTestContext;

        protected override IBasicWindowLessHTMLEngineProvider GetBasicWindowLessHTMLEngineProvider() => new ChromiumFXWindowLessHTMLEngineProvider(MobxTestContext.HtmlProvider);
    }
}
