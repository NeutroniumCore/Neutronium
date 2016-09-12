using Neutronium.Core.Infra;

namespace Tests.Infra.HTMLEngineTesterHelper.HtmlContext
{
    public class ConventionalTestHtmlProvider : ITestHtmlProvider 
    {
        public string GetHtlmPath(TestContext context) 
        {
            return $"{GetType().Assembly.GetPath()}{GetRelativeHtlmPath(context)}";
        }

        public string GetRelativeHtlmPath(TestContext context)
        {
            var relative = context.GetDescription();
            return $"\\TestHtml\\{relative}";
        }
    }
}
