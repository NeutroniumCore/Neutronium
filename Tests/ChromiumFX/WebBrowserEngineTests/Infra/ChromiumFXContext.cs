using Tests.ChromiumFX.Infra;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.ChromiumFX.WebBrowserEngineTests.Infra
{
    public class ChromiumFXContext : ChromiumFXWindowLessHTMLEngineProvider 
    {
        public ChromiumFXContext() : base(new NullTestHtmlProvider())
        {           
        }
    }
}
