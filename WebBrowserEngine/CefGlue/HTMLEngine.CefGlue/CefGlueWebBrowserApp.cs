using Neutronium.WPF;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue
{
    public abstract class CefGlueWebBrowserApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory()
        {
            return new CefGlueWPFWebWindowFactory(GetCefSettings());
        }

        protected virtual CefSettings GetCefSettings()
        {
            return null;
        }
    }
}
