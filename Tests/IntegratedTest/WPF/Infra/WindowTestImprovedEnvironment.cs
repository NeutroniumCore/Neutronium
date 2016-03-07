using System;
using System.Windows;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace IntegratedTest.WPF.Infra 
{
    public abstract class WindowTestImprovedEnvironment : IWindowTestEnvironment 
    {
        private readonly WpfThread _wpfThread;
        private bool _IsInit = false;
        private IWPFWebWindowFactory _WPFWebWindowFactory;

        public WindowTestImprovedEnvironment() 
        {
            _wpfThread = WpfThread.GetWpfThread();
        }

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
                _wpfThread.Dispatcher.Invoke(DoRegister);
                _IsInit = true;
            }
        }

        public IWPFWindowWrapper GetWindowWrapper(Func<Window> ifactory = null) 
        {
            Register();
            return new WPFWindowWrapper(_wpfThread, ifactory);   
        }

        public void Dispose() 
        {
            _wpfThread.Dispatcher.Invoke(() => _WPFWebWindowFactory.Dispose());
            _wpfThread.Dispose();
        }
    }
}
