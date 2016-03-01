using System;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.WPF.Infra
{
    public class WindowTestEnvironment : IDisposable
    {
        public IWPFWebWindowFactory WPFWebWindowFactory { get; set; }

        public IJavascriptUIFrameworkManager FrameworkManager { get; set; }

        public void Register()
        {
            var engine = HTMLEngineFactory.Engine;
            engine.Register(WPFWebWindowFactory);
            engine.Register(FrameworkManager);
        }

        public void Dispose()
        {
            HTMLEngineFactory.Engine.Dispose();
        }
    }
}
