using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace KnockoutUIFramework 
{
    internal class KnockoutUiVmManager : IJavascriptViewModelManager 
    {
        public IJavascriptSessionInjector Injector => _KnockoutSessionInjector;
        public IJavascriptViewModelUpdater ViewModelUpdater { get; }

        private readonly KnockoutSessionInjector _KnockoutSessionInjector;
        private readonly KnockoutViewModelUpdater _KnockoutViewModelUpdater;

        public KnockoutUiVmManager(IWebView webView, IJavascriptObject listener)
        {
            _KnockoutSessionInjector = new KnockoutSessionInjector(webView, listener);
            _KnockoutViewModelUpdater = new KnockoutViewModelUpdater(webView);
        }

        public void Dispose() 
        {
            _KnockoutSessionInjector.Dispose();
            _KnockoutViewModelUpdater.Dispose();
        }
    }
}
