using Neutronium.Core.Infra;

namespace Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext
{
    public class ConventionalTestHtmlProvider : ITestHtmlProvider 
    {
        public string GetHtmlPath(TestContext context) 
        {
            return $"{GetType().Assembly.GetPath()}{GetRelativeHtmlPath(context)}";
        }

        public string GetRelativeHtmlPath(TestContext context)
        {
            var relative = context.GetDescription();
            return $"\\TestHtml\\{relative}";
        }
    }
}
