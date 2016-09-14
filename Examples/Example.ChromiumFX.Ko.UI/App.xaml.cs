using Chromium;
using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Knockout;

namespace Example.ChromiumFX.Ko.UI 
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App  
    {
        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutFrameworkManager();
        }

        protected override void UpdateChromiumSettings(CfxSettings settings)
        {
            settings.RemoteDebuggingPort = 9090;
        }
    }
}
