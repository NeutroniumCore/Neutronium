using CefGlue.TestInfra;
using KnockoutUIFramework.Test.TestHtml;
using UIFrameworkTesterHelper;

namespace CefGlue.Window.Integrated.Tests
{
    public class CefWindowTestEnvironmentKo : CefWindowTestEnvironment
    {
        public override ITestHtmlProvider HtmlProvider => new KnockoutUiHtmlProvider();
    }
}
