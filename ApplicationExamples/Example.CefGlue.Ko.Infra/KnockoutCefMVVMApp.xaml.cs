using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace Example.CefGlue.Ko.Infra
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override IJavascriptUIFrameworkManager GetJavascriptUIFrameworkManager() 
        {
            return new KnockoutUiFrameworkManager();
        }
    }
}
