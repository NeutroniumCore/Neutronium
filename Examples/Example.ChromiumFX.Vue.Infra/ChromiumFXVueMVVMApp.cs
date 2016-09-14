using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Vue;
using Neutronium.WebBrowserEngine.ChromiumFx;

namespace Example.ChromiumFX.Vue.Infra
{
    /// <summary>
    /// Interaction logic for WebBrowserApp.xaml
    /// </summary>
    public class WebBrowserApp : ChromiumFxWebBrowserApp 
    {
        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new VueSessionInjector();
        }
    }
}
