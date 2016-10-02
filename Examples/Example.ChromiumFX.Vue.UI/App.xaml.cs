using Neutronium.JavascriptFramework.Vue;
using Neutronium.WPF;
using System.Windows;

namespace Example.ChromiumFX.Vue.UI
{
    /// <summary>
    /// Interaction logic for CefGlueApp.xaml
    /// </summary>
    public partial class WebBrowserApp
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var engine = HTMLEngineFactory.Engine;
            engine.RegisterJavaScriptFramework(new VueSessionInjectorV2());
            base.OnStartup(e);
        }
    }
}
