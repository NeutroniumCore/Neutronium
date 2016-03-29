using HTML_WPF.Component;

namespace MVVM.Awesomium
{
    public class HTMLAwesomiumApp : HTMLApp
    {
        protected override IWPFWebWindowFactory GetWindowFactory()
        {
           return new AwesomiumWPFWebWindowFactory();
        }
    }
}
