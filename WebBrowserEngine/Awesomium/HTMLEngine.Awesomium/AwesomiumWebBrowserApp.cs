using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.Awesomium
{
    public abstract class AwesomiumWebBrowserApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory()
        {
           return new AwesomiumWPFWebWindowFactory();
        }
    }
}
