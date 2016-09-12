using Neutronium.Core.JavascriptUIFramework;
using Neutronium.JavascriptFramework.Knockout;

namespace Example.CefGlue.Ko.BasicNavigation
{
    /// <summary>
    /// Interaction logic for CefGlueApp.xaml
    /// </summary>
    public partial class CefGlueApp
    {
        protected override IJavascriptUiFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
