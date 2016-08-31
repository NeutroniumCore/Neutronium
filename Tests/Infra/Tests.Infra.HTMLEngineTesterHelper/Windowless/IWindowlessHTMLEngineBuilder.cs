using MVVM.HTML.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.HTMLEngineTesterHelper.Windowless 
{
    public interface IWindowlessHTMLEngineBuilder 
    {
        IWindowlessHTMLEngine Build();

        ITestHtmlProvider HtmlProvider { get; }

        IDispatcher TestUIDispacther { get; }
    }
}
