using Xilium.CefGlue;
using HTML_WPF.Component;

namespace MVVM.Cef.Glue
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
