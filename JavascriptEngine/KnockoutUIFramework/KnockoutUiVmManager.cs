using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace KnockoutUIFramework {
    internal class KnockoutUiVmManager : IJavascriptViewModelManager {
        public IJavascriptSessionInjector Injector { get; }
        public IJavascriptViewModelUpdater ViewModelUpdater { get; }

        public KnockoutUiVmManager(IWebView webView, IJavascriptObject listener) {
            Injector = new KnockoutSessionInjector(webView, listener);
            ViewModelUpdater = new KnockoutViewModelUpdater(webView);
        }
    }
}
