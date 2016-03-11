using System;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.Infra.Windowless 
{
    public class WindowlessTestEnvironment 
    {
        public Func<IJavascriptUIFrameworkManager, IWindowlessJavascriptEngine> WindowlessJavascriptEngineBuilder { get; set; }

        public FrameworkTestContext FrameworkTestContext  {  get; set; }

        public IDispatcher TestUIDispacther { get; set; }

        public IWindowlessJavascriptEngine Build()
        {
            return WindowlessJavascriptEngineBuilder(FrameworkTestContext.FrameworkManager);
        }

        public IJavascriptFrameworkExtractor GetExtractor(IWebView webView) 
        {
            return FrameworkTestContext.JavascriptFrameworkExtractorBuilder(webView);
        }
    }
}
