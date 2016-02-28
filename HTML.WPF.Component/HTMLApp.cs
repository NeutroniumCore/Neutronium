using System.Windows;
using MVVM.HTML.Core.JavascriptUIFramework;
using System.Diagnostics;
using MVVM.HTML.Core.Exceptions;

namespace HTML_WPF.Component
{
    public abstract class HTMLApp : Application
    {
        public IJavascriptUIFrameworkManager JavascriptUiFrameworkManager { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            HTMLEngineFactory.Engine.Register(GetWindowFactory());
            if (JavascriptUiFrameworkManager!=null)
            {
                HTMLEngineFactory.Engine.Register(JavascriptUiFrameworkManager);
            }
            else
            {
                throw ExceptionHelper.Get("No javascript engine registered!! This may result in application crash. Please set JavascriptUiFrameworkManager property before application start");
            }          

            base.OnStartup(e);
        }

        protected abstract IWPFWebWindowFactory GetWindowFactory();

        protected override void OnExit(ExitEventArgs e)
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
