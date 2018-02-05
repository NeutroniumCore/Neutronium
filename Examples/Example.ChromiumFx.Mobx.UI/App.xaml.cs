using Neutronium.WebBrowserEngine.ChromiumFx;
using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.mobx;
using System;
using System.Linq;

namespace Example.ChromiumFx.Mobx.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ChromiumFxWebBrowserApp
    {
        public ApplicationMode Mode { get; private set; }
        public bool RunTimeOnly => (Mode != ApplicationMode.Dev);
        public bool Debug => (Mode != ApplicationMode.Production);
        public Uri Uri => (Mode == ApplicationMode.Dev) ?
                                new Uri("http://localhost:3000/index.html") :
                                new Uri("pack://application:,,,/View/mainview/dist/index.html");
        public static App MainApplication => Current as App;

        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager()
        {
            Mode = GetApplicationMode(Args);
            return new MobxFrameworkManager();
        }

        private static ApplicationMode GetApplicationMode(string[] args) {
#if DEBUG
            var normalizedArgs = args.Select(arg => arg.ToLower()).ToList();
            if (normalizedArgs.Contains("-dev"))
                return ApplicationMode.Dev;

            if (normalizedArgs.Contains("-test"))
                return ApplicationMode.Test;
#endif
            return ApplicationMode.Production;
        }
    }
}
