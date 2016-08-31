using HTMLEngine.CefGlue;
using HTML_WPF.Component;
using KnockoutUIFramework;
using MVVM.HTML.Core.JavascriptUIFramework;
using Tests.Infra.IntegratedContextTesterHelper.Window;

namespace CefGlue.TestInfra
{
    public abstract class CefWindowTestEnvironment : WindowTestEnvironment 
    {
        public override IWPFWebWindowFactory GetWPFWebWindowFactory() 
        {
           return new CefGlueWPFWebWindowFactory();
        }

        public override IJavascriptUIFrameworkManager FrameworkManager => new KnockoutUiFrameworkManager();
    }
}
