using System.Windows;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace HTML_WPF.Component
{
    public abstract class HTMLApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.RegisterHTMLEngine(GetWindowFactory());
            engine.RegisterJavaScriptFramework(GetJavascriptUIFrameworkManager());       
            base.OnStartup(e);
        }

        protected abstract IWPFWebWindowFactory GetWindowFactory();

        protected abstract IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager();

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
