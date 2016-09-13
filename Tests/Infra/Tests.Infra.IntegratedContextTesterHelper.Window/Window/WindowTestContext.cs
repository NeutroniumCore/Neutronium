using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.WPF;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Infra.IntegratedContextTesterHelper.Window
{
    public class WindowTestContext
    {
        public Func<IWPFWebWindowFactory> WPFWebWindowFactory { get; set; }

        public IJavascriptUiFrameworkManager FrameworkManager { get; set; }

        public ITestHtmlProvider HtmlProvider { get; set; }
    }
}
