using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless 
{
    public class WindowlessIntegratedTestEnvironment : IWindowlessIntegratedContextBuilder
    {
        public Func<IWindowlessHTMLEngine> WindowlessJavascriptEngineBuilder { get; set; }

        public FrameworkTestContext FrameworkTestContext  {  get; set; }

        public ITestHtmlProvider HtmlProvider => FrameworkTestContext?.HtmlProvider;

        public IDispatcher TestUIDispacther { get; set; }

        public IWindowlessHTMLEngine Build()
        {
            return WindowlessJavascriptEngineBuilder();
        }

        public IJavascriptFrameworkExtractor GetExtractor(IWebView webView) 
        {
            return FrameworkTestContext.JavascriptFrameworkExtractorBuilder(webView);
        }
    }
}
