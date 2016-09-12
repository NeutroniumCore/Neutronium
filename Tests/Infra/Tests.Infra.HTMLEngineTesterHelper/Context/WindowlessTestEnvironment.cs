using Neutronium.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace Tests.Infra.HTMLEngineTesterHelper.Context 
{
    public interface IWindowlessTestEnvironment 
    {
        ITestHtmlProvider HtmlProvider { get; }

        IDispatcher TestUIDispacther { get; set; }

        IWindowlessHTMLEngine Build();
    }
}
