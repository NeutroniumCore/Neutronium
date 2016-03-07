using System;
using System.Windows;
using System.Windows.Threading;

namespace IntegratedTest.WPF.Infra
{
    public class WindowTest : IDisposable
    {
        private readonly WPFThreadingHelper _WPFThreadingHelper;

        public WindowTest(Action<Window> Init)
        {
            _WPFThreadingHelper = new WPFThreadingHelper(
                () =>
                {
                    var window = new Window();
                    NameScope.SetNameScope(window, new NameScope());
                    Init(window);
                    return window;
                } );
        }

        public Window Window { get { return _WPFThreadingHelper.MainWindow; } }

        public Dispatcher Dispatcher { get { return Window.Dispatcher; } }

        public void RunOnUIThread(Action Do)
        {
            Dispatcher.Invoke(Do);
        }

        public void Dispose()
        {   
            Action End = () => { _WPFThreadingHelper.CloseWindow(); };
            Dispatcher.Invoke(End);
            _WPFThreadingHelper.Dispose();
        }
    }
}
