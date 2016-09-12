using HTMEngine.ChromiumFX;
using Neutronium.Core.JavascriptUIFramework;
using Neutronium.JavascriptFramework.Vue;

namespace Example.ChromiumFX.Vue.Infra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App : ChromiumFXApp 
    {
        protected override IJavascriptUiFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new VueSessionInjector();
        }
    }
}
