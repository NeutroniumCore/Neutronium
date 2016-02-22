using System.Windows;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace HTML_WPF.Component
{
    public abstract class HTMLApp : Application
    {
        protected IJavascriptSessionInjectorFactory JavascriptSessionInjectorFactory { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            HTMLEngineFactory.Engine.Register(GetWindowFactory());
            HTMLEngineFactory.Engine.Register(JavascriptSessionInjectorFactory);

            base.OnStartup(e);
        }

        protected abstract IWPFWebWindowFactory GetWindowFactory();

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
