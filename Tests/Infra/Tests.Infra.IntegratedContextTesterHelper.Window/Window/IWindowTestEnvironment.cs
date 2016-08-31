using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.Infra.IntegratedContextTesterHelper.Window
{
    public interface IWindowTestEnvironment : IWindowTestHTMLEngineEnvironment
    {
        IJavascriptUIFrameworkManager FrameworkManager { get;}
        IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null);
        ITestHtmlProvider HtmlProvider { get; }
    }
}
