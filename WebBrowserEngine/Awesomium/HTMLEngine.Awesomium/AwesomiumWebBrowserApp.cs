using Neutronium.Core;
using Neutronium.WPF;

namespace Neutronium.WebBrowserEngine.Awesomium
{
    public abstract class AwesomiumWebBrowserApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory(IWebSessionLogger logger)
        {
           return new AwesomiumWPFWebWindowFactory(logger);
        }
    }
}
