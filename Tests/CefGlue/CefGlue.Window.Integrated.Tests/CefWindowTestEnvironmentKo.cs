using CefGlue.TestInfra;
using KnockoutUIFramework.Test.TestHtml;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace CefGlue.Window.Integrated.Tests
{
    public class CefWindowTestEnvironmentKo : CefWindowTestEnvironment
    {
        public override ITestHtmlProvider HtmlProvider => new KnockoutUiHtmlProvider();
    }
}
