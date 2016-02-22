using KnockoutUIFramework;

namespace KnockoutCefMVVMApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            JavascriptSessionInjectorFactory = new KnockoutSessionInjectorFactory();
        }
    }
}
