using System;
using System.Windows;
using System.Windows.Threading;

namespace IntegratedTest.WPF.Infra
{
    public class WPFWindowTestWrapper
    {
        public Window Window { get; private set; }

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> OnException;

        public WPFWindowTestWrapper(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window", "You must initialize WPFWindowTestWrapper before you can update the window layout.");

            Window = window;
            Window.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }
       
        public void ShowWindow()
        {
            Window.Show();
            Window.BringIntoView();
        }

        public void Close()
        {
            if (Window == null)
                return;

            Window.Close();
            Window.Dispatcher.UnhandledException -= OnDispatcherUnhandledException;
            Window = null;
            GC.Collect();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (OnException != null)
                OnException(this, e);
        }
    }   
}


