using System;
using System.Windows;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.WPF
{
    public abstract class HTMLApp : Application
    {
        private bool _Registered;

        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.RegisterHTMLEngine(GetWindowFactory());
            engine.RegisterJavaScriptFramework(GetJavascriptUIFrameworkManager());
            OnStartUp(engine);
            base.OnStartup(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (_Registered)
                return;

            _Registered = true;
            MainWindow.Closing += Closing;
        }

        private void Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
        }

        protected virtual void OnStartUp(IHTMLEngineFactory factory) 
        {            
        } 

        protected abstract IWPFWebWindowFactory GetWindowFactory();

        protected abstract IJavascriptFrameworkManager GetJavascriptUIFrameworkManager();
    }
}
