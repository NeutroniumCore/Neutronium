using Neutronium.Core;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptUIFramework;

namespace KnockoutUIFramework 
{
    internal class KnockoutUiVmManager : IJavascriptViewModelManager 
    {
        public IJavascriptSessionInjector Injector => _KnockoutSessionInjector;
        public IJavascriptViewModelUpdater ViewModelUpdater => _KnockoutViewModelUpdater;

        private readonly KnockoutSessionInjector _KnockoutSessionInjector;
        private readonly KnockoutViewModelUpdater _KnockoutViewModelUpdater;
        private readonly IWebSessionLogger _Logger;

        public KnockoutUiVmManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger)
        {
            _Logger = logger;
            _KnockoutSessionInjector = new KnockoutSessionInjector(webView, listener, _Logger);
            _KnockoutViewModelUpdater = new KnockoutViewModelUpdater(webView);
        }

        public void Dispose() 
        {
            _KnockoutSessionInjector.Dispose();
            _KnockoutViewModelUpdater.Dispose();
        }
    }
}
