using HTML_WPF.Component;
using MVVM.Awesomium;
using MVVM.Cef.Glue;
using System.Windows;

namespace MVVM_Awesonium_UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            HTMLEngineFactory.Engine.Register(new AwesomiumWPFWebWindowFactory() );
            HTMLEngineFactory.Engine.Register(new CefGlueWPFWebWindowFactory());
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
            base.OnExit(e);
        }
    }
}
