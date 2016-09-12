using HTML_WPF.Component;
using System;
using Neutronium.Core.JavascriptUIFramework;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.IntegratedContextTesterHelper.Window
{
    public class WindowTestContext
    {
        public Func<IWPFWebWindowFactory> WPFWebWindowFactory { get; set; }

        public IJavascriptUiFrameworkManager FrameworkManager { get; set; }

        public ITestHtmlProvider HtmlProvider { get; set; }
    }
}
