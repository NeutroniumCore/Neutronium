using IntegratedTest;
using KnockoutUIFramework.Test.TestHelper;
using MVVM.Cef.Glue.Test.Generic;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Cef.Glue.Test.Core
{
    public abstract class MVVMCefGlue_Test_Base : MVVMCefCore_Test_Base
    {
        public MVVMCefGlue_Test_Base() : base(GetEnvironment())
        {
        }

        private static TestEnvironment GetEnvironment()
        {
            return new TestEnvironment()
            {
                WindowlessJavascriptEngineBuilder = () => new CefGlueWindowlessJavascriptEngine(),
                JavascriptFrameworkExtractorBuilder = (webView) => new KnockoutExtractor(webView),
                TestUIDispacther = new NullUIDispatcher()
            };
        }
    }
}
