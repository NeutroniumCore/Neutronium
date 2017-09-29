using Neutronium.Core;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.JavascriptFramework.Knockout 
{
    internal class KnockoutUiVmManager : IJavascriptViewModelManager 
    {
        public IJavascriptSessionInjector Injector => _KnockoutSessionInjector;
        public IJavascriptViewModelUpdater ViewModelUpdater => _KnockoutViewModelUpdater;

        private readonly KnockoutSessionInjector _KnockoutSessionInjector;
        private readonly KnockoutViewModelUpdater _KnockoutViewModelUpdater;

        public KnockoutUiVmManager(IWebView webView, IJavascriptObject listener, IWebSessionLogger logger)
        {
            _KnockoutSessionInjector = new KnockoutSessionInjector(webView, listener, logger);
            _KnockoutViewModelUpdater = new KnockoutViewModelUpdater(webView, logger);
        }

        public void Dispose() 
        {
            _KnockoutSessionInjector.Dispose();
            _KnockoutViewModelUpdater.Dispose();
        }
    }
}
