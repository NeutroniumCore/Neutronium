using CefGlue.TestInfra;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.CefGlue.HTLMEngineTests.Infra
{
    public class CefGlueContext : CefGlueWindowlessSharedJavascriptEngineFactory 
    {
        public CefGlueContext() : base(new NullTestHtmlProvider())
        {           
        }
    }
}
