using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace IntegratedTest.WPF.Infra
{
    public class WindowTest : IDisposable
    {
        private readonly IWPFWindowWrapper _WPFThreadingHelper;
        private IDispatcher _Dispatcher;

        public WindowTest(IWindowTestEnvironment context, Action<Window> init)
        {
            _WPFThreadingHelper = context.GetWindowWrapper(() => CreateNewWindow(init));
        }

        private Window CreateNewWindow(Action<Window> init)
        {
            var window = new Window();
            NameScope.SetNameScope(window, new NameScope());
            init(window);
            _Dispatcher = new WPFUIDispatcher(window.Dispatcher);
            return window;
        }

        public Window Window { get { return _WPFThreadingHelper.MainWindow; } }

        public Dispatcher Dispatcher { get { return Window.Dispatcher; } }

        public async Task RunOnUIThread(Action Do) 
        {
            await _Dispatcher.RunAsync(Do);
        }

        public async Task<T> EvaluateOnUIThread<T>(Func<T> Do) 
        {
            return await _Dispatcher.EvaluateAsync(Do);
        }

        public IDispatcher GetDispatcher() 
        {
            return _Dispatcher;
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
