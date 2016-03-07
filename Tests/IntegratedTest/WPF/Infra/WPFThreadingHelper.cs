using System;
using System.Threading;
using System.Windows;

namespace IntegratedTest.WPF.Infra
{
    internal class WPFThreadingHelper : IWPFWindowWrapper
    {
        private readonly Func<Window> _factory;
        private WPFWindowTestWrapper _wpfWindowTestWrapper;
        private readonly WpfThread _wpfThread;

        public Thread UIThread
        {
            get { return _wpfThread.UIThread; }
        }

        public Window MainWindow
        {
            get { return _wpfWindowTestWrapper.Window; }
        }

        public WPFThreadingHelper(Func<Window> ifactory = null, WpfThread wpfThread=null)
        {
            _wpfThread = wpfThread ?? new WpfThread();
            Func<Window> basic =() => new Window();
            _factory = ifactory ?? basic;

            _wpfThread.Dispatcher.Invoke(InitWindow);
        }

        private void InitWindow()
        {
            _wpfWindowTestWrapper = new WPFWindowTestWrapper(_factory());
            _wpfWindowTestWrapper.ShowWindow();
        }

        public void CloseWindow()
        {
            _wpfWindowTestWrapper.CloseWindow();
        }

        public void Dispose()
        {
            _wpfThread.Dispose();
        }
    }
}
