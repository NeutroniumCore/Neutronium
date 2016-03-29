using HTML_WPF.Component;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue
{
    public class HTMLCefGlueApp : HTMLApp
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
