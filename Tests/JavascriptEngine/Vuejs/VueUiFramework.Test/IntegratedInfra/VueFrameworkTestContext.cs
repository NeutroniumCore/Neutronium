using Tests.Infra.JavascriptEngineTesterHelper;
using VueUiFramework.Test.TestHtml;

namespace VueUiFramework.Test.IntegratedInfra
{
    public class VueFrameworkTestContext
    {
        public static FrameworkTestContext GetVueFrameworkTestContext()
        {
            return new FrameworkTestContext
            {
                JavascriptFrameworkExtractorBuilder = (webView) => new VueExtractor(webView),
                FrameworkManager = new VueSessionInjector(),
                HtmlProvider = new VueUiHtmlProvider()
            };
        }
    }
}
