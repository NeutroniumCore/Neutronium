using System;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Window 
{
    public interface IWindowTestHTMLEngineEnvironment : IDisposable 
    {
        IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null);

        ITestHtmlProvider HtmlProvider { get; }
    }
}
