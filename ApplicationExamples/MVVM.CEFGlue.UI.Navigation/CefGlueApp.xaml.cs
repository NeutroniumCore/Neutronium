using HTMLEngine.CefGlue;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Cef.Glue.UI.Navigation
{
    /// <summary>
    /// Interaction logic for CefGlueApp.xaml
    /// </summary>
    public partial class CefGlueApp : HTMLCefGlueApp
    {
        protected override IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
