using Chromium.Remote;
using HTMEngine.ChromiumFX.EngineBinding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.Convertion
{
    public static class ChromiumFXWebViewExtension
    {
        public static CfrFrame Convert(this IWebView context)
        {
            var chromiumContext = context as ChromiumFXWebView;
            return (chromiumContext == null) ? null : chromiumContext.GetRaw();
        }

        public static IWebView Convert(this CfrFrame context)
        {
            return new ChromiumFXWebView(context.Browser);
        }
    }
}
