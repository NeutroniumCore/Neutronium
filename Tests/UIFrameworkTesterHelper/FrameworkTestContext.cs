using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace UIFrameworkTesterHelper
{
    public class FrameworkTestContext 
    {
        public IJavascriptUIFrameworkManager FrameworkManager { get; set; }

        public Func<IWebView, IJavascriptFrameworkExtractor> JavascriptFrameworkExtractorBuilder { get; set; }

        public ITestHtmlProvider HtmlProvider { get; set; }
    }
}
