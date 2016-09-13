using Tests.Awesomium.Infra;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Awesomium.HTMLEngineTests.Infra
{
    public class AwesomiumContext : AwesomiumEngineProvider 
    {
        public AwesomiumContext() : base(new NullTestHtmlProvider())
        {           
        }
    }
}
