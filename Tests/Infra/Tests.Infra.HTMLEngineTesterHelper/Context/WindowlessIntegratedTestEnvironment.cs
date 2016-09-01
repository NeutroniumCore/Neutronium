using System;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace Tests.Infra.HTMLEngineTesterHelper.Context 
{
    public class WindowlessIntegratedTestEnvironment : IWindowlessHTMLEngineBuilder
    {
        public Func<IWindowlessHTMLEngine> WindowlessJavascriptEngineBuilder { get; set; }
        public ITestHtmlProvider HtmlProvider { get; set; }
        public IDispatcher TestUIDispacther { get; set; }

        public IWindowlessHTMLEngine Build()
        {
            return WindowlessJavascriptEngineBuilder();
        }
    }
}
