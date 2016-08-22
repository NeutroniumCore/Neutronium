using HTMLEngine.CefGlue;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace Example.CefGlue.Ko.Infra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class App : HTMLCefGlueApp 
    {
        protected override IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
