using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace KnockoutCefMVVMApp
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
