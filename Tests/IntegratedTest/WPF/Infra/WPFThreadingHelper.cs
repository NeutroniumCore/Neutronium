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
            var thread = new Thread(InitUIinSTA) {
                Name = "Simulated UI Thread"
            };
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();

            _ARE.WaitOne();
            _UIThread = MainWindow.Dispatcher.Thread;
        }

        public void Close()
        {
            _wpfWindowTestWrapper.Close();
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
