using Mobx.Test.TestHtml;
using Neutronium.JavascriptFramework.mobx;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace Mobx.Test.IntegratedInfra
{
    public class MobxFrameworkTestContext
    {
        public static FrameworkTestContext GetMobxFrameworkTestContext()
        {
            return new FrameworkTestContext
            {
                JavascriptFrameworkExtractorBuilder = (webView) => new MobxExtractor(webView),
                FrameworkManager = new MobxFrameworkManager(),
                HtmlProvider = new MobxHtmlProvider()
            };
        }
    }
}
