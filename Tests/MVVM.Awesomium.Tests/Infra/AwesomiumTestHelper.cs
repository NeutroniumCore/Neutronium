using IntegratedTest;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.Tests.Infra
{
    public static class AwesomiumTestHelper
    {
        //private static AwesomiumWPFWebWindowFactory _AwesomiumWPFWebWindowFactory;
        //static AwesomiumTestHelper()
        //{
        //    _AwesomiumWPFWebWindowFactory = new AwesomiumWPFWebWindowFactory();
        //}
        public static IJavascriptUIFrameworkManager GetUIFrameworkManager()
        {
            return new KnockoutUiFrameworkManager();
        }

        public static WindowTestEnvironment GetWindowEnvironment()
        {
            return new WindowTestEnvironment()
            {
                FrameworkManager = GetUIFrameworkManager(),
                WPFWebWindowFactory = new AwesomiumWPFWebWindowFactory()
            };
        }

        public static WindowlessTestEnvironment GetWindowlessEnvironment()
        {
            return new WindowlessTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = (frameWork) => new AwesomiumWindowlessJavascriptEngine(frameWork),
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                FrameworkManager = GetUIFrameworkManager(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }
    }
}
