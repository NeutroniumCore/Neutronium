using System;
using MVVM.HTML.Core.JavascriptUIFramework;
using UIFrameworkTesterHelper;

namespace IntegratedTest.Infra.Window
{
    public interface IWindowTestEnvironment : IDisposable
    {
        IJavascriptUIFrameworkManager FrameworkManager { get;}
        IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null);
        ITestHtmlProvider HtmlProvider { get; }
    }
}
