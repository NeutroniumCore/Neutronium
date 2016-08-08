using System;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.Infra.Window 
{
    public interface IWindowTestEnvironment : IDisposable
    {
        IJavascriptUIFrameworkManager FrameworkManager { get;}
        IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null);
        ITestHtmlProvider HtmlProvider { get; }
    }
}
