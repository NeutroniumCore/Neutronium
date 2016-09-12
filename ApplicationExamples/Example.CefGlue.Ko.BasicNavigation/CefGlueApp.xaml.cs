using KnockoutUIFramework;
using Neutronium.Core.JavascriptUIFramework;

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
