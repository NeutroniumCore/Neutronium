using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Knockout;

namespace Example.CefGlue.Ko.BasicNavigation
{
    /// <summary>
    /// Interaction logic for CefGlueWebBrowserApp.xaml
    /// </summary>
    public partial class CefGlueWebBrowserApp
    {
        protected override IJavascriptUiFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
