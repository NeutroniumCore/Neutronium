using Neutronium.WPF;

namespace HTMLEngine.Awesomium
{
    public abstract class HTMLAwesomiumApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory()
        {
           return new AwesomiumWPFWebWindowFactory();
        }
    }
}
