using System.Windows;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using System;

namespace Neutronium.WPF.Internal
{
    public class WPFHTMLWindowProvider : IWebBrowserWindowProvider
    {
        private readonly UIElement _UIElement;
        private readonly Neutronium.WPF.Internal.HTMLControlBase _HTMLControlBase;
        private readonly IWPFWebWindow _WPFWebWindow;

        public IWebBrowserWindow HTMLWindow => _WPFWebWindow.HTMLWindow;
        public IWPFWebWindow WPFWebWindow => _WPFWebWindow;
        public IDispatcher UIDispatcher => new WPFUIDispatcher(_UIElement.Dispatcher);
        public event EventHandler<DebugEventArgs> DebugToolOpened
        {
            add { _WPFWebWindow.DebugToolOpened += value; }
            remove { _WPFWebWindow.DebugToolOpened -= value; }
        }
        public event EventHandler OnDisposed;

        public WPFHTMLWindowProvider(IWPFWebWindow wpfWebWindow, Neutronium.WPF.Internal.HTMLControlBase htmlControlBase)
        {
            _WPFWebWindow = wpfWebWindow;
            _HTMLControlBase = htmlControlBase;
            _UIElement = _WPFWebWindow.UIElement;
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
            return _WPFWebWindow.OnDebugToolsRequest();
        }

        public void CloseDebugTools() 
        {
            _WPFWebWindow.CloseDebugTools();
        }

        public void Dispose()
        {
            _UIElement.Visibility = Visibility.Hidden;
            _HTMLControlBase.MainGrid.Children.Remove(_UIElement);

            _WPFWebWindow.Dispose();

            OnDisposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
