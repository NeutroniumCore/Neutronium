using System;
using System.Windows;
using Neutronium.WebBrowserEngine.ChromiumFx;
using System.Diagnostics;

namespace Example.ChromiumFX.Vue.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new object();
            wcBrowser.OnDisplay += WcBrowser_OnDisplay;        
        }

        private void WcBrowser_OnDisplay(object sender, Neutronium.Core.Navigation.DisplayEvent e)
        {
            var windowCfx = wcBrowser.WPFWebWindow as IWPFCfxWebWindow;
            windowCfx.ChromiumWebBrowser.DragHandler.OnDragEnter += DragHandler_OnDragEnter;
        }

        private void DragHandler_OnDragEnter(object sender, Chromium.Event.CfxOnDragEnterEventArgs e)
        {
            Trace.WriteLine("OnDragEnter fired");
        }

        protected override void OnClosed(EventArgs e)
        {
            this.wcBrowser.Dispose();
            base.OnClosed(e);
        }
    }
}
