using System.Windows;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using System;

namespace Neutronium.WPF.Internal
{
    public class WPFHTMLWindowProvider : IWebBrowserWindowProvider
    {
        private readonly UIElement _UIElement;
        private readonly HTMLControlBase _HTMLControlBase;

        public IWebBrowserWindow HtmlWindow => WPFWebWindow.HTMLWindow;
        public IWPFWebWindow WPFWebWindow { get; }
        public IUiDispatcher UiDispatcher => new WPFUIDispatcher(_UIElement.Dispatcher);
        public event EventHandler<DebugEventArgs> DebugToolOpened;
        public event EventHandler OnDisposed;

        public WPFHTMLWindowProvider(IWPFWebWindow wpfWebWindow, Neutronium.WPF.Internal.HTMLControlBase htmlControlBase)
        {
            WPFWebWindow = wpfWebWindow;
            _HTMLControlBase = htmlControlBase;
            _UIElement = WPFWebWindow.UIElement;
            WPFWebWindow.DebugToolOpened += _WPFWebWindow_DebugToolOpened;
        }

        private void _WPFWebWindow_DebugToolOpened(object sender, DebugEventArgs e)
        {
            var debugToolOpened = DebugToolOpened;
            if (debugToolOpened != null)
            {
                UiDispatcher.Dispatch(() => debugToolOpened(this, e));
            }
        }

        public void Show()
        {
            _UIElement.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _UIElement.Visibility = Visibility.Hidden;
        }

        public bool OnDebugToolsRequest() 
        {
            return WPFWebWindow.OnDebugToolsRequest();
        }

        public void CloseDebugTools() 
        {
            WPFWebWindow.CloseDebugTools();
        }

        public void Dispose()
        {
            _UIElement.Visibility = Visibility.Hidden;
            _HTMLControlBase.MainGrid.Children.Remove(_UIElement);

            WPFWebWindow.Dispose();

            OnDisposed?.Invoke(this, EventArgs.Empty);
            WPFWebWindow.DebugToolOpened -= _WPFWebWindow_DebugToolOpened;
        }
    }
}
