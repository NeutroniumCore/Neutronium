using System.Windows;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.WPF
{
    public abstract class HTMLApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.RegisterHTMLEngine(GetWindowFactory());
            engine.RegisterJavaScriptFramework(GetJavascriptUIFrameworkManager());
            OnStartUp(engine);
            base.OnStartup(e);
        }

        protected virtual void OnStartUp(IHTMLEngineFactory factory) 
        {            
        }

        protected abstract IWPFWebWindowFactory GetWindowFactory();

        protected abstract IJavascriptFrameworkManager GetJavascriptUIFrameworkManager();

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
