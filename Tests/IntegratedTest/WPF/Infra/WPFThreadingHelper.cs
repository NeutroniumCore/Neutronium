using System;
using System.Threading;
using System.Windows;
using IntegratedTest.WPF.Infra;

namespace Integrated.WPFInfra
{
    internal class WPFThreadingHelper : IDisposable
    {
        private readonly Thread _UIThread;
        private readonly Func<Window> _factory;
        private readonly AutoResetEvent _ARE;
        private WPFWindowTestWrapper _wpfWindowTestWrapper;
        private CancellationTokenSource _CTS;

        internal Thread UIThread
        {
            get { return _UIThread; }
        }

        internal Window MainWindow
        {
            get { return _wpfWindowTestWrapper.Window; }
        }

        public WPFThreadingHelper(Func<Window> ifactory=null )
        {
            Func<Window> basic =() => new Window();
            _factory = ifactory ?? basic;
            _CTS = new CancellationTokenSource();
            _ARE = new AutoResetEvent(false);
            _UIThread = new Thread(InitUIinSTA) {
                Name = "Simulated UI Thread"
            };
            _UIThread.SetApartmentState(ApartmentState.STA);
            _UIThread.Start();

            _ARE.WaitOne();
            //_UIThread = MainWindow.Dispatcher.Thread;
        }

        public void CloseWindow()
        {
            _wpfWindowTestWrapper.CloseWindow();
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
            _wpfWindowTestWrapper = new WPFWindowTestWrapper(_factory());        
            _wpfWindowTestWrapper.ShowWindow();
            _ARE.Set();

            while (_CTS.IsCancellationRequested == false)
            {
                DispatcherHelper.DoEvents();
            }
        }
    }
}
