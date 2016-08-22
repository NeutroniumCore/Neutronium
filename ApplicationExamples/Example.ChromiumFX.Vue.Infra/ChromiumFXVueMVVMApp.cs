using HTMEngine.ChromiumFX;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace Example.ChromiumFX.Vue.Infra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ChromiumFXApp 
    {
        protected override IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new VueUiFramework.VueSessionInjector();
        }
    }
}
