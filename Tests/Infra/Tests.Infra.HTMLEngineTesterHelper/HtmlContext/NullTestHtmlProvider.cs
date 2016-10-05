using Neutronium.Core.Infra;

namespace Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext
{
    public class NullTestHtmlProvider : ITestHtmlProvider 
    {
        public string GetHtmlPath(TestContext context)
        {
            return $"{GetType().Assembly.GetPath()}\\Html\\Empty.html";
        }

        public string GetRelativeHtmlPath(TestContext context)
        {
            return "Html\\Empty.html";
        }
    }
}
