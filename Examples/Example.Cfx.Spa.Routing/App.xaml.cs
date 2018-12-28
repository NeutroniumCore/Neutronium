using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Vue;
using Neutronium.WebBrowserEngine.ChromiumFx;
using Neutronium.WPF;
using System.Windows;
using Example.Cfx.Spa.Routing.SetUp;

namespace Example.Cfx.Spa.Routing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ChromiumFxWebBrowserApp
    {
        public static SetUpViewModel SetUp => (Current as App)?.SetUpViewModel;

        private SetUpViewModel SetUpViewModel { get; }
        private readonly ApplicationSetUpBuilder _ApplicationSetUpBuilder;

        public App()
        {
            _ApplicationSetUpBuilder = new ApplicationSetUpBuilder("View");
            SetUpViewModel = new SetUpViewModel(_ApplicationSetUpBuilder);
        }

        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager()
        {
            return new VueSessionInjector();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _ApplicationSetUpBuilder.Dispose();
            base.OnExit(e);
        }

        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
#if DEBUG
            SetUpViewModel.InitFromArgs(Args).Wait();
#else
            SetUpViewModel.InitForProduction();
#endif

            factory.RegisterJavaScriptFrameworkAsDefault(new VueSessionInjectorV2 { RunTimeOnly = true });
            base.OnStartUp(factory);
        }
    }
}
