using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MVVM.CEFGlue.Test
{
    public static class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
    internal class WPFThreadingHelper : IDisposable
    {

        private Thread _UIThread;
        private AutoResetEvent _ARE;
        private Window _window;
        private WPFTester _wpfTester;
        private CancellationTokenSource _CTS;

        internal Thread UIThread
        {
            get { return _UIThread; }
        }

        internal Window MainWindow
        {
            get { return _window; }
        }

        private Func<Window> _factory;

        public WPFThreadingHelper(Func<Window> ifactory=null)
        {
            Func<Window> basic =() => new Window();
            _factory = ifactory ?? basic;
            _CTS = new CancellationTokenSource();
            _ARE = new AutoResetEvent(false);
            Thread thread = new Thread(InitUIinSTA);
            thread.Name = "Simulated UI Thread";
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();

            _ARE.WaitOne();
            _UIThread = _window.Dispatcher.Thread;
        }

        public void Close()
        {
            _wpfTester.Close();
        }

        public void Dispose()
        {
            _CTS.Cancel();
            _UIThread.Join();
            _ARE.Dispose();
            _ARE = null;
            _CTS.Dispose();
            _CTS = null;
        }

        private void InitUIinSTA()
        {
            _wpfTester = new WPFTester();
            var application = new Application();
            _window = _factory();
            _wpfTester.ShowWindow(_window);
            _ARE.Set();

            while (_CTS.IsCancellationRequested == false)
            {
                DispatcherHelper.DoEvents();
            }
        }
    }
}
