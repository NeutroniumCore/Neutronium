using System;
using Neutronium.Core.WebBrowserEngine.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Context 
{
    public class WindowlessIntegratedTestEnvironment : IWindowlessHTMLEngineBuilder
    {
        public Func<IWindowlessHTMLEngine> WindowlessJavascriptEngineBuilder { get; set; }
        public ITestHtmlProvider HtmlProvider { get; set; }
        public IUiDispatcher TestUIDispacther { get; set; }

        public IWindowlessHTMLEngine Build()
        {
            return WindowlessJavascriptEngineBuilder();
        }
    }
}
