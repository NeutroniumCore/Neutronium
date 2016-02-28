using IntegratedTest;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.Cef.Glue.Test.Generic;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Cef.Glue.Test.Infra
{
    public static class CefTestHelper
    {
        public static IJavascriptUIFrameworkManager GetUIFrameworkManager()
        {
            return new KnockoutUiFrameworkManager();
        }

        public static WindowTestEnvironment GetEnvironment()
        {
            return new WindowTestEnvironment()
            {
                FrameworkManager = GetUIFrameworkManager(),
                WPFWebWindowFactory = new CefGlueWPFWebWindowFactory()
            };
        }

        public static WindowlessTestEnvironment GetWindowEnvironment()
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
