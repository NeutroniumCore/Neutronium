using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Convertion
{
    public static class ChromiumFxWebViewExtension
    {
        public static CfrFrame Convert(this IWebView context)
        {
            var chromiumContext = context as ChromiumFxWebView;
            return chromiumContext?.GetRaw();
        }
    }
}
