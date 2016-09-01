using Tests.ChromiumFX.Infra;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.ChromiumFX.HTLMEngineTests.Infra
{
    public class ChromiumFXContext : ChromiumFXWindowLessHTMLEngineProvider 
    {
        public ChromiumFXContext() : base(new NullTestHtmlProvider())
        {           
        }
    }
}
