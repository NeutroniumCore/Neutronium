using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.IntegratedContextTesterHelper.Window
{
    public class WindowTestContext
    {
        public Func<IWPFWebWindowFactory> WPFWebWindowFactory { get; set; }

        public IJavascriptUIFrameworkManager FrameworkManager { get; set; }

        public ITestHtmlProvider HtmlProvider { get; set; }
    }
}
