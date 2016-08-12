using IntegratedTest.JavascriptUIFramework;
using KnockoutUIFramework.Test.TestHtml;

namespace VueUiFramework.Test.IntegratedInfra
{
    public class VueFrameworkTestContext
    {
        public static FrameworkTestContext GetKnockoutFrameworkTestContext()
        {
            return new FrameworkTestContext
            {
                JavascriptFrameworkExtractorBuilder = (webView) => new VueExtractor(webView),
                FrameworkManager = new VueSessionInjector(),
                HtmlProvider = new KnockoutUiHtmlProvider()
            };
        }
    }
}
