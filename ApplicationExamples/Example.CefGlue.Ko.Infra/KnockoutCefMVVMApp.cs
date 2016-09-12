using HTMLEngine.CefGlue;
using Neutronium.Core.JavascriptFramework;
using Neutronium.JavascriptFramework.Knockout;

namespace Example.CefGlue.Ko.Infra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App : HTMLCefGlueApp 
    {
        protected override IJavascriptUiFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
