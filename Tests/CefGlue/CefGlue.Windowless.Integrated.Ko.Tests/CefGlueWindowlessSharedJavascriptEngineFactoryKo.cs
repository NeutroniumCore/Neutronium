using CefGlue.TestInfra;
using IntegratedTest.JavascriptUIFramework;
using KnockoutUIFramework.Test.IntegratedInfra;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryKo : CefGlueWindowlessSharedJavascriptEngineFactory
    {
        protected override FrameworkTestContext FrameworkTestContext => KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
    }
}
