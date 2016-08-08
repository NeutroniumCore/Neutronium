using System;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.Infra;

namespace KnockoutUIFramework.Test.TestHtml 
{
    public class KnockoutUiHtmlProvider : ITestHtmlProvider
    {
        public string GetHtlmPath(TestContext context, bool allowInitialScriptInjection) 
        {
            switch (context) 
            {
                case TestContext.SimpleNavigation:
                    return $"{GetType().Assembly.GetPath()}\\TestHtml\\Navigation data\\index.html";

                case TestContext.Navigation1:
                    return $"{GetType().Assembly.GetPath()}\\TestHtml\\javascript\\navigation_1.html";

                case TestContext.Navigation2:
                    return $"{GetType().Assembly.GetPath()}\\TestHtml\\javascript\\navigation_2.html";
            }
            throw new NotImplementedException();
        }
    }
}
