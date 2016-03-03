using IntegratedTest;
using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.Tests.Infra 
{
    public class AwesomiumTestContext : IHTMLWindowProviderTestContext 
    {
        private AwesomiumWindowlessSharedJavascriptEngine _AwesomiumWindowlessSharedJavascriptEngine;

        public IJavascriptUIFrameworkManager GetUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }

        private AwesomiumWindowlessSharedJavascriptEngine GetWindowLessEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager) 
        {
            if (_AwesomiumWindowlessSharedJavascriptEngine != null)
                return _AwesomiumWindowlessSharedJavascriptEngine;

            return _AwesomiumWindowlessSharedJavascriptEngine = new AwesomiumWindowlessSharedJavascriptEngine(javascriptUIFrameworkManager);
        }

        public WindowTestEnvironment GetWindowEnvironment() 
        {
            return new WindowTestEnvironment() 
            {
                FrameworkManager = GetUIFrameworkManager(),
                WPFWebWindowFactory = () => new AwesomiumWPFWebWindowFactory()
            };
        }

        public WindowlessTestEnvironment GetWindowlessEnvironment() 
        {
            return new WindowlessTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = (frameWork) => GetWindowLessEngine(frameWork),
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                FrameworkManager = GetUIFrameworkManager(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }

        public void Dispose() 
        {
            if (_AwesomiumWindowlessSharedJavascriptEngine != null)
            {
                _AwesomiumWindowlessSharedJavascriptEngine.Dispose();
            }
        }
    }
}
