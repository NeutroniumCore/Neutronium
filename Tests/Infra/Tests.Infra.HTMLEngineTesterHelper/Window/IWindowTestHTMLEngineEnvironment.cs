using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.HTMLEngineTesterHelper.Window 
{
    public interface IWindowTestHTMLEngineEnvironment : IDisposable 
    {
        IJavascriptUIFrameworkManager FrameworkManager { get; }
        IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null);
        ITestHtmlProvider HtmlProvider { get; }
    }
}
