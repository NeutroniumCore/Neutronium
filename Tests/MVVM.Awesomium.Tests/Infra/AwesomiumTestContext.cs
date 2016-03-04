using IntegratedTest;
using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.Tests.Infra 
{
    public class AwesomiumTestContext : IWindowLessHTMLEngineProvider 
    {
        private AwesomiumWindowlessHTMLEngineFactory _AwesomiumWindowlessHTMLEngineFactory;


        public IJavascriptUIFrameworkManager GetUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }

        private AwesomiumWindowlessHTMLEngineFactory GetWindowLessEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager) 
        {
            if (_AwesomiumWindowlessHTMLEngineFactory != null)
                return _AwesomiumWindowlessHTMLEngineFactory;

            return _AwesomiumWindowlessHTMLEngineFactory = new AwesomiumWindowlessHTMLEngineFactory(javascriptUIFrameworkManager);
        }

        public WindowlessTestEnvironment GetWindowlessEnvironment() 
        {
            return new WindowlessTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = (frameWork) => GetWindowLessEngine(frameWork).CreateWindowlessJavascriptEngine(),
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                FrameworkManager = GetUIFrameworkManager(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }

        public void Dispose() 
        {
            if (_AwesomiumWindowlessHTMLEngineFactory != null)
            {
                _AwesomiumWindowlessHTMLEngineFactory.Dispose();
            }
        }
    }
}
