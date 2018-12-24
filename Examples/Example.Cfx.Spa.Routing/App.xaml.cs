using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Vue;
using Neutronium.WebBrowserEngine.ChromiumFx;
using Neutronium.WPF;
using System;
using Example.Cfx.Spa.Routing.SetUp;

namespace Example.Cfx.Spa.Routing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ChromiumFxWebBrowserApp
    {
        public bool? Debug => _ApplicationSetUp?.Debug;
        public Uri Uri => _ApplicationSetUp?.Uri;

        private readonly ApplicationSetUpBuilder _ApplicationSetUpBuilder;
        private ApplicationSetUp _ApplicationSetUp;

        public static App MainApplication => Current as App;

        public App()
        {
            var uri = new Uri("pack://application:,,,/View/dist/index.html");
            _ApplicationSetUpBuilder = new ApplicationSetUpBuilder(uri);
        }

        protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager()
        {
            return new VueSessionInjector();
        }

        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
#if DEBUG
            _ApplicationSetUp = _ApplicationSetUpBuilder.BuildFromApplicationArguments(Args);
#else
            _ApplicationSetUp = _ApplicationSetUpBuilder.BuildForProduction();
#endif
            factory.RegisterJavaScriptFrameworkAsDefault(new VueSessionInjectorV2 { RunTimeOnly = true });
            base.OnStartUp(factory);
        }
    }
}
