using Chromium.Event;
using Neutronium.JavascriptFramework.Vue;
using Neutronium.WPF;

namespace Example.ChromiumFx.Vue.Chromeless
{
    /// <summary>
    /// Interaction logic for CefGlueApp.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartUp(IHTMLEngineFactory factory)
        {
            factory.RegisterJavaScriptFramework(new VueSessionInjectorV2 {RunTimeOnly = true});
            base.OnStartUp(factory);
        }

        protected override void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
        {
            beforeLineCommand.CommandLine.AppendSwitch("disable-web-security");
        }
    }
}
