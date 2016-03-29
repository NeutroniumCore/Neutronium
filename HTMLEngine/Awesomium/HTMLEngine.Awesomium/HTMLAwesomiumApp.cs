using HTML_WPF.Component;

namespace HTMLEngine.Awesomium
{
    public class HTMLAwesomiumApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory()
        {
           return new AwesomiumWPFWebWindowFactory();
        }
    }
}
