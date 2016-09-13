using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Infra.JavascriptFrameworkTesterHelper
{
    public class FrameworkTestContext 
    {
        public IJavascriptFrameworkManager FrameworkManager { get; set; }

        public Func<IWebView, IJavascriptFrameworkExtractor> JavascriptFrameworkExtractorBuilder { get; set; }

        public ITestHtmlProvider HtmlProvider { get; set; }
    }
}
