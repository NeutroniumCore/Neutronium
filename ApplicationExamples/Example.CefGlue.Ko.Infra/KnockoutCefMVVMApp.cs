using HTMLEngine.CefGlue;
using KnockoutUIFramework;
using Neutronium.Core.JavascriptUIFramework;

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
