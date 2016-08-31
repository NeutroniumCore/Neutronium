using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless 
{
    public interface IWindowlessIntegratedContextBuilder : IWindowlessHTMLEngineBuilder
    {
        FrameworkTestContext FrameworkTestContext { get; set; }

        IJavascriptFrameworkExtractor GetExtractor(IWebView webView);
    }
}
