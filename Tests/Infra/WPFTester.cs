using MVVM.HTML.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Xunit;

namespace MVVM.Cef.Glue.Test
{
   

    public class WPFTester
    {
        #region Fields

        private static readonly DispatcherOperationCallback _exitFrameCallback;

        #endregion

        #region Constructor

        static WPFTester()
        {
            _exitFrameCallback = OnExitFrame;
        }

        #endregion

        #region Properties

        public bool IsInitialized
        {
            get { return (this.Window != null); }
        }

        public Window Window { get; private set; }

        #endregion

        #region Methods

        public void ShowWindow(Window window)
        {
            Initialize(window);
            ShowWindow();
        }

        public void ShowControl(FrameworkElement control)
        {
            Initialize(new Window());

            this.Window.Content = control;
            ShowWindow();
        }

        public void ShowControl(FrameworkElement control, ResourceDictionary dictionary)
        {
            Initialize(new Window());

            this.Window.Resources = dictionary;
            this.Window.Content = control;

            ShowWindow();
        }

        public void Close()
        {
            if (this.Window == null)
                return;

            this.Window.Close();
            this.Window.Dispatcher.UnhandledException -= OnDispatcherUnhandledException;
            this.Window = null;
            GC.Collect();
        }

        public void UpdateWindowLayout()
        {
            if (!this.IsInitialized)
                throw new Exception("You must initialize WPFTester before you can update the window layout.");

            this.Window.UpdateLayout();
        }

        private void Initialize(Window window)
        {
            if (this.IsInitialized)
                return;

            if (window == null)
                throw new ArgumentNullException("window", "You must initialize WPFTester before you can update the window layout.");

            this.Window = window;
            this.Window.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void ShowWindow()
        {
            this.Window.Show();
            this.Window.BringIntoView();
        }

        public static bool ShouldReceivedError=false;

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!ShouldReceivedError)
                Assert.False(true,e.Exception.ToString());
        }

        #endregion

        #region DoEvents

        public static void DoEvents()
        {
            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // The priority of this callback should be lower than the that of UI event messages.
            DispatcherOperation exitOperation =
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                                                        _exitFrameCallback, nestedFrame);

            // Pump the nested message loop, the nested message loop will
            // immediately process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "OnExitFrame" callback doesn't get finished, Abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object OnExitFrame(Object state)
        {
            var frame = state as DispatcherFrame;
            if (frame != null)
            {
                // Exit the nested message loop.
                frame.Continue = false;
            }
            return null;
        }

        #endregion

    }   
}


