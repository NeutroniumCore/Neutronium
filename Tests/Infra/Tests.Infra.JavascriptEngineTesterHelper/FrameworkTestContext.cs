using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.JavascriptEngineTesterHelper
{
    public class FrameworkTestContext 
    {
        public IJavascriptUiFrameworkManager FrameworkManager { get; set; }

        public Func<IWebView, IJavascriptFrameworkExtractor> JavascriptFrameworkExtractorBuilder { get; set; }

        public ITestHtmlProvider HtmlProvider { get; set; }
    }
}
