using System;
using System.Windows;
using System.Windows.Threading;

namespace IntegratedTest.WPF.Infra
{
    public class WindowTest : IDisposable
    {
        private readonly IWPFWindowWrapper _WPFThreadingHelper;

        public WindowTest(Action<Window> init) 
        {
            _WPFThreadingHelper = new WPFThreadingHelper(() => CreateNewWindow(init));
        }

        public WindowTest(IWindowTestEnvironment context, Action<Window> init) 
        {
            _WPFThreadingHelper = context.GetWindowWrapper(() => CreateNewWindow(init));
        }

        private Window CreateNewWindow(Action<Window> init) 
        {
            var window = new Window();
            NameScope.SetNameScope(window, new NameScope());
            init(window);
            return window;
        }

        public Window Window { get { return _WPFThreadingHelper.MainWindow; } }

        public Dispatcher Dispatcher { get { return Window.Dispatcher; } }

        public void RunOnUIThread(Action Do)
        {
            Dispatcher.Invoke(Do);
        }

        public void CloseWindow() 
        {
            _WPFThreadingHelper.CloseWindow();
        }

        public void Dispose()
        {   
            Action End = () => { _WPFThreadingHelper.CloseWindow(); };
            Dispatcher.Invoke(End);
            _WPFThreadingHelper.Dispose();
        }
    }
}
