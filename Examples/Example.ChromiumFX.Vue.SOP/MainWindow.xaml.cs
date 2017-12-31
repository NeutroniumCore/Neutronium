using System;
using System.Windows;
using Neutronium.WebBrowserEngine.ChromiumFx;
using System.Diagnostics;
using Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding;

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
            var cfxBrowser = windowCfx.ChromiumWebBrowser;
            cfxBrowser.DragHandler.OnDragEnter += DragHandler_OnDragEnter;
            cfxBrowser.ContextMenuHandler.OnBeforeContextMenu += ContextMenuHandler_OnBeforeContextMenu;
        }

        private void ContextMenuHandler_OnBeforeContextMenu(object sender, Chromium.Event.CfxOnBeforeContextMenuEventArgs e)
        {
            var model = e.Model;
            model.InsertItemAt(0,(int)ContextMenuId.MENU_ID_PRINT, "Print");
            model.InsertSeparatorAt(1);
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
