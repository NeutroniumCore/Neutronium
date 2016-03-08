using System;
using System.Windows;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.WPF.Infra 
{
    public abstract class WindowTestImprovedEnvironment : IWindowTestEnvironment 
    {
        private bool _IsInit = false;
        private IWPFWebWindowFactory _WPFWebWindowFactory;

        protected WindowTestImprovedEnvironment() 
        {
        }

        public WpfThread WpfThread { get; private set; }

        public abstract IWPFWebWindowFactory GetWPFWebWindowFactory();

        public abstract IJavascriptUIFrameworkManager FrameworkManager { get; }

        private void DoRegister() 
        {

            var engine = HTMLEngineFactory.Engine;
            _WPFWebWindowFactory = GetWPFWebWindowFactory();
            engine.Register(_WPFWebWindowFactory);
            engine.Register(FrameworkManager);
        }

        private void Register() 
        {
            if (!_IsInit) 
            {
                WpfThread = WpfThread.GetWpfThread();
                WpfThread.AddRef();
                WpfThread.Dispatcher.Invoke(DoRegister);
                WpfThread.OnThreadEnded += OnThreadEnded;
                _IsInit = true;
            }
        }

        private void OnThreadEnded(object sender, EventArgs e)
        {
            _WPFWebWindowFactory.Dispose();
        }

        public IWPFWindowWrapper GetWindowWrapper(Func<Window> ifactory = null) 
        {
            Register();
            return new WPFWindowWrapper(WpfThread, ifactory);   
        }

        public void Dispose() 
        {
            WpfThread.Release();
        }
    }
}
