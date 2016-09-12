using System;
using HTML_WPF.Component;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.Infra.IntegratedContextTesterHelper.Window
{
    public sealed class WindowTestEnvironment : IWindowTestHTMLEngineEnvironment
    {
        private bool _IsInit = false;
        private IWPFWebWindowFactory _WPFWebWindowFactory;
        private readonly WindowTestContext _WindowTestContext;

        public WpfThread WpfThread { get; private set; }

        //public abstract IWPFWebWindowFactory GetWPFWebWindowFactory();
        //public abstract IJavascriptUiFrameworkManager FrameworkManager { get; }

        public ITestHtmlProvider HtmlProvider => _WindowTestContext.HtmlProvider;

        public WindowTestEnvironment(WindowTestContext context) 
        {
            _WindowTestContext = context;
        }

        private void DoRegister() 
        {
            var engine = HTMLEngineFactory.Engine;
            _WPFWebWindowFactory = _WindowTestContext.WPFWebWindowFactory();
            engine.RegisterHTMLEngine(_WPFWebWindowFactory);
            engine.RegisterJavaScriptFramework(_WindowTestContext.FrameworkManager);
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
