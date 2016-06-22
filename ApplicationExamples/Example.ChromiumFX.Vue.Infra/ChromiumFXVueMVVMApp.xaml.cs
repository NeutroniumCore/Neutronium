using MVVM.HTML.Core.JavascriptUIFramework;

namespace Example.ChromiumFX.Vue.Infra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new VueUiFramework.VueSessionInjector();
        }
    }
}
