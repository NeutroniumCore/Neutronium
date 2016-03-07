using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace IntegratedTest.WPF.Infra
{
    public class WpfThread : IDisposable
    {
        private readonly Thread _UIThread;
        private readonly AutoResetEvent _ARE;
        private readonly CancellationTokenSource _CTS;
        private Dispatcher _dispatcher;

        public Thread UIThread
        {
            get { return _UIThread; }
        }

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        public WpfThread()
        {
            _CTS = new CancellationTokenSource();
            _ARE = new AutoResetEvent(false);
            _UIThread = new Thread(InitUIinSTA) {
                Name = "Simulated UI Thread"
            };
            _UIThread.SetApartmentState(ApartmentState.STA);
            _UIThread.Start();

            _ARE.WaitOne();
        }

        public void Dispose()
        {
            _CTS.Cancel();
            _UIThread.Join();
            _ARE.Dispose();
            _CTS.Dispose();
        }

        private void InitUIinSTA()
        {
            var fmel = new FrameworkElement();
            _dispatcher = fmel.Dispatcher;
            _ARE.Set();

            while (_CTS.IsCancellationRequested == false)
            {
                DispatcherHelper.DoEvents();
            }
        }
    }
}
