using System;
using System.Windows.Threading;

namespace IntegratedTest.Infra.Window
{
    public class WPFWindowTestWrapper
    {
        public System.Windows.Window Window { get; private set; }

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> OnException;

        public WPFWindowTestWrapper(System.Windows.Window window)
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

        public void CloseWindow()
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
            OnException?.Invoke(this, e);
        }
    }   
}


