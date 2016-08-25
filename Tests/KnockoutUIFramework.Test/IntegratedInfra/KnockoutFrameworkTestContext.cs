using KnockoutUIFramework.Test.TestHtml;
using UIFrameworkTesterHelper;

namespace KnockoutUIFramework.Test.IntegratedInfra
{
    public static class KnockoutFrameworkTestContext
    {
        public static FrameworkTestContext GetKnockoutFrameworkTestContext() 
        {
            return new FrameworkTestContext 
            {
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                FrameworkManager = new KnockoutUiFrameworkManager(),
                HtmlProvider = new KnockoutUiHtmlProvider()
            };
        }
    }
}
