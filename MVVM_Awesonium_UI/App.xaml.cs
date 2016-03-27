using HTML_WPF.Component;
using MVVM.Awesomium;
using MVVM.Cef.Glue;
using System.Windows;
using HTMEngine.ChromiumFX;
using KnockoutUIFramework;

namespace MVVM_Awesonium_UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.Register(new AwesomiumWPFWebWindowFactory() );
            engine.Register(new CefGlueWPFWebWindowFactory());
            engine.Register(new KnockoutUiFrameworkManager());
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
            base.OnExit(e);
        }
    }
}
