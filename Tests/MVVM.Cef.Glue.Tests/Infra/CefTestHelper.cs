using IntegratedTest;
using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Cef.Glue.Tests.Infra
{
    public static class CefTestHelper
    {
        public static IJavascriptUIFrameworkManager GetUIFrameworkManager()
        {
            return new KnockoutUiFrameworkManager();
        }

        public static WindowTestEnvironment GetWindowEnvironment()
        {
            return new WindowTestEnvironment()
            {
                FrameworkManager = GetUIFrameworkManager(),
                WPFWebWindowFactory = () => new CefGlueWPFWebWindowFactory()
            };
        }

        public static WindowlessTestEnvironment GetWindowlessEnvironment()
        {
            return new WindowlessTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = (frameWork) => new CefGlueWindowlessJavascriptEngine(frameWork),
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                FrameworkManager = GetUIFrameworkManager(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }
    }
}
