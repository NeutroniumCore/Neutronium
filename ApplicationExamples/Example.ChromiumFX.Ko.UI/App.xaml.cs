using Chromium;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace Example.ChromiumFX.Ko.UI 
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App  
    {
        protected override IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }

        protected override void UpdateChromiumSettings(CfxSettings settings)
        {
            settings.RemoteDebuggingPort = 9090;
        }
    }
}
