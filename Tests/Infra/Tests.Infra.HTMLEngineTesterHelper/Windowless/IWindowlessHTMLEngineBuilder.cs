using Neutronium.Core.WebBrowserEngine.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Windowless 
{
    public interface IWindowlessHTMLEngineBuilder 
    {
        IWindowlessHTMLEngine Build();

        ITestHtmlProvider HtmlProvider { get; }

        IDispatcher TestUIDispacther { get; }
    }
}
