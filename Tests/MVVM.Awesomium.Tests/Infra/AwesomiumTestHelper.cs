using IntegratedTest;
using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using KnockoutUIFramework.Test.IntegratedInfra;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.Tests.Infra
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
                WPFWebWindowFactory = () => new AwesomiumWPFWebWindowFactory()
            };
        }

        public static WindowlessTestEnvironment GetWindowlessEnvironment()
        {
            return new WindowlessTestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = (frameWork) => new AwesomiumWindowlessJavascriptEngine(frameWork),
                FrameworkTestContext = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }
    }
}
