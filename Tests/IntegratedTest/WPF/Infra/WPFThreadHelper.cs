using System;
using System.Threading;
using System.Windows;

namespace IntegratedTest.WPF.Infra
{
    internal class WPFThreadingHelper : IDisposable
    {
        private readonly Func<Window> _factory;
        private WPFWindowTestWrapper _wpfWindowTestWrapper;
        private WPFThreadHelper _WPFThreadHelper;

        internal Thread UIThread
        {
            get { return _WPFThreadHelper.UIThread; }
        }

        internal Window MainWindow
        {
            get { return _wpfWindowTestWrapper.Window; }
        }

        public WPFThreadingHelper(Func<Window> ifactory = null, WPFThreadHelper wpfThreadHelper=null)
        {
            _WPFThreadHelper = wpfThreadHelper ?? new WPFThreadHelper();
            Func<Window> basic =() => new Window();
            _factory = ifactory ?? basic;

            _WPFThreadHelper.Dispatcher.Invoke(InitWindow);
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
            _WPFThreadHelper.Dispose();
        }
    }
}
