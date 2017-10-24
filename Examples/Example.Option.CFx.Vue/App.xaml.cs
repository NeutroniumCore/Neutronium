using Neutronium.JavascriptFramework.Vue;
using Neutronium.WPF;

namespace Example.Option.CFx.Vue 
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
            factory.RegisterJavaScriptFramework(new VueSessionInjectorV2());
            base.OnStartUp(factory);
        }
    }
}
