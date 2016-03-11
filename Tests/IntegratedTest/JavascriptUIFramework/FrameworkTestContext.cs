using System;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.JavascriptUIFramework 
{
    public class FrameworkTestContext 
    {
        public IJavascriptUIFrameworkManager FrameworkManager { get; set; }

        public Func<IWebView, IJavascriptFrameworkExtractor> JavascriptFrameworkExtractorBuilder { get; set; }
    }
}
