using Neutronium.JavascriptFramework.Vue;
using Tests.Infra.JavascriptFrameworkTesterHelper;
using VueFramework.Test.TestHtml;

namespace VueFramework.Test.IntegratedInfra
{
    public class VueFrameworkTestContext
    {
        public static FrameworkTestContext GetVueFrameworkTestContext()
        {
            return new FrameworkTestContext
            {
                JavascriptFrameworkExtractorBuilder = (webView) => new VueExtractor(webView),
                FrameworkManager = new VueSessionInjector(),
                HtmlProvider = new VueHtmlProvider()
            };
        }
    }
}
