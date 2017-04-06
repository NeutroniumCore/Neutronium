using System;
using System.Windows;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Infra;
using System.Windows.Markup;

namespace Neutronium.WPF
{
    public abstract class HTMLApp : Application
    {
        protected HTMLApp() 
        {
            AddResource();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.RegisterHTMLEngine(GetWindowFactory());
            engine.RegisterJavaScriptFramework(GetJavascriptUIFrameworkManager());
            OnStartUp(engine);
            base.OnStartup(e);
        }

        private void AddResource()
        {
            var reader = new ResourceReader("Windows", typeof(HTMLApp).Assembly);
            var myResourceDictionary = (ResourceDictionary)XamlReader.Parse(reader.Load("ResourceDictionary.xaml"));
            Resources.MergedDictionaries.Add(myResourceDictionary);
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
