using System;
using System.Windows;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.WPF.Infra 
{
    public interface IWindowTestEnvironment : IDisposable
    {
        WpfThread WpfThread { get; set; }
        IJavascriptUIFrameworkManager FrameworkManager { get;}
        IWPFWindowWrapper GetWindowWrapper(Func<Window> ifactory = null);
    }
}
