using System;
using System.Linq;
using Chromium.Event;
using Neutronium.JavascriptFramework.Vue;
using Neutronium.WPF;

namespace Example.ChromiumFx.Vue.Chromeless
{
    /// <summary>
    /// Interaction logic for CefGlueApp.xaml
    /// </summary>
    public partial class App
    {
        public ApplicationMode Mode { get; private set; }
        public bool RunTimeOnly => (Mode != ApplicationMode.Dev);
        public bool Debug => (Mode != ApplicationMode.Production);
        public Uri Uri => (Mode == ApplicationMode.Dev) ? 
                                new Uri("http://localhost:91/index.html") : 
                                new Uri("pack://application:,,,/View/dist/index.html");

        public static App MainApplication => Current as App;

        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
            Mode = GetApplicationMode(Args);
            factory.RegisterJavaScriptFramework(new VueSessionInjectorV2{ RunTimeOnly = RunTimeOnly });
            base.OnStartUp(factory);
        }

        private static ApplicationMode GetApplicationMode(string[] args)
        {
            var normalizedArgs = args.Select(arg => arg.ToLower()).ToList();
            if (normalizedArgs.Contains("-dev"))
                return ApplicationMode.Dev;

            if (normalizedArgs.Contains("-test"))
                return ApplicationMode.Test;

            return ApplicationMode.Production;
        }

        protected override void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            beforeLineCommand.CommandLine.AppendSwitch("disable-web-security");
        }
    }
}
