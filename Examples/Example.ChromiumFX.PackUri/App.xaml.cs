using Neutronium.JavascriptFramework.Vue;
using Neutronium.WPF;

namespace Example.ChromiumFX.PackUri
{
    /// <summary>
    /// Interaction logic for CefGlueApp.xaml
    /// </summary>
    public partial class WebBrowserApp
    {
        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
            factory.RegisterJavaScriptFramework(new VueSessionInjectorV2());
            base.OnStartUp(factory);
        }
    }
}
