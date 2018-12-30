using Chromium;
using Chromium.WebBrowser;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Util
{
    public static class NeutroniumSettings
    {
        private static CfxBrowserSettings _NeutroniumBrowserSettings;

        internal static CfxBrowserSettings NeutroniumBrowserSettings
        {
            get
            {
                if (_NeutroniumBrowserSettings != null)
                    return _NeutroniumBrowserSettings;

                return _NeutroniumBrowserSettings = Clone(ChromiumWebBrowser.DefaultBrowserSettings);
            }
        }

        private static CfxBrowserSettings Clone(CfxBrowserSettings original)
        {
            return new CfxBrowserSettings
            {
                Webgl = original.Webgl,
                ApplicationCache = original.ApplicationCache,
                Databases = original.Databases,
                LocalStorage = original.LocalStorage,
                TabToLinks = original.TabToLinks,
                TextAreaResize = original.TextAreaResize,
                ImageShrinkStandaloneToFit = original.ImageShrinkStandaloneToFit,
                ImageLoading = original.ImageLoading,
                WebSecurity = original.WebSecurity,
                FileAccessFromFileUrls = original.FileAccessFromFileUrls,
                UniversalAccessFromFileUrls = original.UniversalAccessFromFileUrls,
                Plugins = original.Plugins,
                JavascriptDomPaste = original.JavascriptDomPaste,
                JavascriptAccessClipboard = original.JavascriptAccessClipboard,
                JavascriptCloseWindows = original.JavascriptCloseWindows,
                Javascript = original.Javascript,
                RemoteFonts = original.RemoteFonts,
                DefaultEncoding = original.DefaultEncoding,
                MinimumLogicalFontSize = original.MinimumLogicalFontSize,
                MinimumFontSize = original.MinimumFontSize,
                DefaultFixedFontSize = original.DefaultFixedFontSize,
                DefaultFontSize = original.DefaultFontSize,
                FantasyFontFamily = original.FantasyFontFamily,
                CursiveFontFamily = original.CursiveFontFamily,
                SansSerifFontFamily = original.SansSerifFontFamily,
                SerifFontFamily = original.SerifFontFamily,
                FixedFontFamily = original.FixedFontFamily,
                StandardFontFamily = original.StandardFontFamily,
                WindowlessFrameRate = original.WindowlessFrameRate,
                BackgroundColor = original.BackgroundColor,
                AcceptLanguageList = original.AcceptLanguageList
            };
        }
    }
}
