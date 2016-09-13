using CefGlue.TestInfra;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.CefGlue.HTMLEngineTests.Infra
{
    public class CefGlueContext : CefGlueWindowlessSharedJavascriptEngineFactory 
    {
        public CefGlueContext() : base(new NullTestHtmlProvider())
        {           
        }
    }
}
