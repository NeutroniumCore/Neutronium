using System;
using MVVM.HTML.Core.Infra;

namespace UIFrameworkTesterHelper
{
    public class ConventionalTestHtmlProvider : ITestHtmlProvider 
    {
        public ConventionalTestHtmlProvider()
        {
        }

        public string GetHtlmPath(TestContext context) 
        {
            switch (context) 
            {
                case TestContext.Index:
                    return BuildPath("javascript\\index.html");

                case TestContext.Simple:
                    return BuildPath("javascript\\simple.html");

                case TestContext.EmptyWithJs:
                    return BuildPath("javascript\\empty_with_js.html");

                case TestContext.AlmostEmpty:
                    return BuildPath("javascript\\almost_empty.html");

                case TestContext.IndexPromise:
                    return BuildPath("javascript\\index_promise.html");

                case TestContext.SimpleNavigation:
                    return BuildPath("Navigation data\\index.html");

                case TestContext.Navigation1:
                    return BuildPath("javascript\\navigation_1.html");

                case TestContext.Navigation2:
                    return BuildPath("javascript\\navigation_2.html");

                case TestContext.Navigation3:
                    return BuildPath("javascript\\navigation_3.html");
            }
            throw new NotImplementedException();
        }

        private string BuildPath(string relative) 
        {
            return $"{GetType().Assembly.GetPath()}\\TestHtml\\{relative}";
        }
    }
}
