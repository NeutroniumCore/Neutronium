using Chromium;
using Chromium.Remote.Event;
using KnockoutUIFramework;

namespace MVVM.ChromiumFX 
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App  
    {
        public App() 
        {
            JavascriptUiFrameworkManager = new KnockoutUiFrameworkManager();
        }

        protected override void UpdateChromiumSettings(CfxSettings settings)
        {
            settings.RemoteDebuggingPort = 9090;
        }
    }
}
