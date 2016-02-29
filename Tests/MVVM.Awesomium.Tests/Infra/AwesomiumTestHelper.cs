using IntegratedTest;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.Awesomium;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.Awesomium.Tests;

namespace MVVM.Cef.Glue.Test.Infra
{
    public static class AwesomiumTestHelper
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
