using HTMLEngine.CefGlue;
using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Knockout;
using Neutronium.WebBrowserEngine.CefGlue;

namespace Example.CefGlue.Ko.Infra
{
    /// <summary>
    /// Interaction logic for WebBrowserApp.xaml
    /// </summary>
    public class WebBrowserApp : CefGlueWebBrowserApp 
    {
        protected override IJavascriptUiFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
