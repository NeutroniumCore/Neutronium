using System;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptUIFramework;
using UIFrameworkTesterHelper;

namespace IntegratedTest.Infra.Window
{
    public abstract class WindowTestEnvironment : IWindowTestEnvironment 
    {
        private bool _IsInit = false;
        private IWPFWebWindowFactory _WPFWebWindowFactory;

        public WpfThread WpfThread { get; private set; }

        public abstract IWPFWebWindowFactory GetWPFWebWindowFactory();

        public abstract IJavascriptUIFrameworkManager FrameworkManager { get; }

        public abstract ITestHtmlProvider HtmlProvider { get; }

        private void DoRegister() 
        {
            var engine = HTMLEngineFactory.Engine;
            _WPFWebWindowFactory = GetWPFWebWindowFactory();
            engine.RegisterHTMLEngine(_WPFWebWindowFactory);
            engine.RegisterJavaScriptFramework(FrameworkManager);
        }

        private void Register()
        {
            if (_IsInit)
                return;

            WpfThread = WpfThread.GetWpfThread();
            WpfThread.AddRef();
            WpfThread.Dispatcher.Invoke(DoRegister);
            WpfThread.OnThreadEnded += OnThreadEnded;
            _IsInit = true;
        }

        private void OnThreadEnded(object sender, EventArgs e)
        {
            _WPFWebWindowFactory.Dispose();
        }

        public IWPFWindowWrapper GetWindowWrapper(Func<System.Windows.Window> ifactory = null) 
        {
            Register();
            return new WPFWindowWrapper(WpfThread, ifactory);   
        }

        public void Dispose() 
        {
            WpfThread?.Release();
        }
    }
}
