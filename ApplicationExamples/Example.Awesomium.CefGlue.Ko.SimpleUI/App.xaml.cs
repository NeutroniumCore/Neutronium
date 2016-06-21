using System.Windows;
using HTMLEngine.Awesomium;
using HTMLEngine.CefGlue;
using HTML_WPF.Component;
using KnockoutUIFramework;

namespace Example.Awesomium.CefGlue.Ko.SimpleUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.RegisterHTMLEngine(new AwesomiumWPFWebWindowFactory() );
            engine.RegisterHTMLEngine(new CefGlueWPFWebWindowFactory());
            engine.RegisterJavaScriptFramework(new KnockoutUiFrameworkManager());
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
            base.OnExit(e);
        }
    }
}
