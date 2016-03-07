using IntegratedTest.WPF.Infra;
using KnockoutUIFramework;
using MVVM.Cef.Glue;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace CefGlue.TestInfra
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
    }
}
