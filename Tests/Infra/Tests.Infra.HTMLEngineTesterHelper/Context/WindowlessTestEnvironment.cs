using Neutronium.Core.WebBrowserEngine.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Context 
{
    public interface IWindowlessTestEnvironment 
    {
        ITestHtmlProvider HtmlProvider { get; }

        IDispatcher TestUIDispacther { get; set; }

        IWindowlessHTMLEngine Build();
    }
}
