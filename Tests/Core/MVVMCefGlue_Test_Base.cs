using IntegratedTest;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.Cef.Glue.Test.Generic;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Cef.Glue.Test.Core
{
    public abstract class MVVMCefGlue_Test_Base : MVVMCefCore_Test_Base
    {
        protected override IWindowlessJavascriptEngine GetWindowlessJavascriptEngine()
        {
            return new CefGlueWindowlessJavascriptEngine();
        }

        protected override IJavascriptFrameworkExtractor BuildJavascriptFrameworkExtractor(IWebView webview)
        {
            return new KnockoutExtractor(webview);
        }
    }
}
