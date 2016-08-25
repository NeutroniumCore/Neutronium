using CefGlue.TestInfra;
using KnockoutUIFramework.Test.IntegratedInfra;
using UIFrameworkTesterHelper;

namespace CefGlue.Windowless.Integrated.Ko.Tests
{
    public class CefGlueWindowlessSharedJavascriptEngineFactoryKo : CefGlueWindowlessSharedJavascriptEngineFactory
    {
        protected override FrameworkTestContext FrameworkTestContext => KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext();
    }
}
