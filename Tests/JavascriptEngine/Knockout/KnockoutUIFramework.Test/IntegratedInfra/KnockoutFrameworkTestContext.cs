using KnockoutFramework.Test.TestHtml;
using Neutronium.JavascriptFramework.Knockout;
using Tests.Infra.JavascriptFrameworkTesterHelper;

namespace KnockoutFramework.Test.IntegratedInfra
{
    public static class KnockoutFrameworkTestContext
    {
        public static FrameworkTestContext GetKnockoutFrameworkTestContext() 
        {
            return new FrameworkTestContext 
            {
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                FrameworkManager = new KnockoutUiFrameworkManager(),
                HtmlProvider = new KnockoutHtmlProvider()
            };
        }
    }
}
