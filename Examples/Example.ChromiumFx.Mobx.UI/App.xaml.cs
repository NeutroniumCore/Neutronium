using Neutronium.WebBrowserEngine.ChromiumFx;
using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.mobx;

namespace Example.ChromiumFx.Mobx.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ChromiumFxWebBrowserApp
    {
        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager()
        {
            return new MobxFrameworkManager();
        }
    }
}
