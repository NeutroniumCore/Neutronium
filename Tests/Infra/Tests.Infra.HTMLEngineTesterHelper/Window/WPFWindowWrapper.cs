using System;
using System.Threading;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Window
{
    public class WPFWindowWrapper : IWPFWindowWrapper
    {
        private readonly Func<System.Windows.Window> _factory;
        private WPFWindowTestWrapper _wpfWindowTestWrapper;
        private readonly WpfThread _wpfThread;

        public Thread UIThread => _wpfThread.UIThread;

        public System.Windows.Window MainWindow => _wpfWindowTestWrapper.Window;

        public WPFWindowWrapper(WpfThread wpfThread, Func<System.Windows.Window> ifactory = null) 
        {
            _wpfThread = wpfThread;
            Func<System.Windows.Window> basic =() => new System.Windows.Window();
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
            CloseWindow();
        }
    }
}
