using CefGlue.TestInfra;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.CefGlue.WebBrowserEngineTests.Infra
{
    public class CefGlueContext : CefGlueWindowlessSharedJavascriptEngineFactory 
    {
        public CefGlueContext() : base(new NullTestHtmlProvider())
        {           
        }
    }
}
