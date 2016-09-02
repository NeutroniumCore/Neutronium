using System;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.HTMLEngineTesterHelper.Window 
{
    public interface IWindowTestHTMLEngineEnvironment : IDisposable 
    {
        IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null);

        ITestHtmlProvider HtmlProvider { get; }
    }
}
