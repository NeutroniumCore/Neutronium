using Neutronium.Core;
using Neutronium.WPF;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue
{
    public abstract class CefGlueWebBrowserApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory(IWebSessionLogger logger)
        {
            return new CefGlueWPFWebWindowFactory(logger, GetCefSettings());
        }

        protected virtual CefSettings GetCefSettings()
        {
            return null;
        }
    }
}
