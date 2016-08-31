using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Tests.Infra.HTMLEngineTesterHelper.Window
{
    public class WpfThread : IDisposable
    {
        private readonly Thread _UIThread;
        private readonly AutoResetEvent _ARE;
        private readonly CancellationTokenSource _CTS;
        private Dispatcher _dispatcher;
        private int _Count;

        public event EventHandler OnThreadEnded;

        public Thread UIThread => _UIThread;

        public Dispatcher Dispatcher => _dispatcher;

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

        private static readonly Lazy<WpfThread> _LazyWpfThread = new Lazy<WpfThread>(() => new WpfThread());		
        public static WpfThread GetWpfThread()
        {		
            return _LazyWpfThread.Value;		
        }

        public void AddRef() 
        {
            _Count++;
        }

        public void Release() 
        {
            if (--_Count==0)
                Dispose();            
        }

        public void Dispose()
        {
            _CTS.Cancel();
            _UIThread.Join();
            _ARE.Dispose();
            _CTS.Dispose();
        }

        private void FireEnded()
        {
            var threadEnded = OnThreadEnded;
            threadEnded?.Invoke(this, EventArgs.Empty);
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

            FireEnded();
        }
    }
}
