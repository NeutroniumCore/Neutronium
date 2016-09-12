using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WPF.Internal;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.Infra.IntegratedContextTesterHelper.Window
{
    public class WindowTest : IDisposable
    {
        private readonly IWPFWindowWrapper _WPFThreadingHelper;
        private WindowTestEnvironment _WindowTestEnvironment;
        private IDispatcher _Dispatcher;

        public WindowTest(WindowTestContext context, Action<System.Windows.Window> init)
        {
            _WindowTestEnvironment = new WindowTestEnvironment(context);
            _WPFThreadingHelper = _WindowTestEnvironment.GetWindowWrapper(() => CreateNewWindow(init));
        }

        private System.Windows.Window CreateNewWindow(Action<System.Windows.Window> init)
        {
            var window = new System.Windows.Window();
            NameScope.SetNameScope(window, new NameScope());
            init(window);
            _Dispatcher = new WPFUIDispatcher(window.Dispatcher);
            return window;
        }

        public System.Windows.Window Window => _WPFThreadingHelper.MainWindow;

        public Dispatcher Dispatcher => Window.Dispatcher;

        public async Task RunOnUIThread(Action Do)
        {
            await _Dispatcher.RunAsync(Do);
        }

        public async Task RunOnUIThread(Func<Task> Do)
        {
            await await EvaluateOnUIThread(Do);
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
            Action end = () => { _WPFThreadingHelper.CloseWindow(); };
            Dispatcher.Invoke(end);
            _WPFThreadingHelper.Dispose();
            _WindowTestEnvironment.Dispose();
        }
    }
}
