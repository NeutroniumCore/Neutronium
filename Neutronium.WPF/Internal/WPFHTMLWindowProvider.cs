using System.Windows;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.WPF.Internal
{
    public class WPFHTMLWindowProvider : IWebBrowserWindowProvider
    {
        private readonly UIElement _UIElement;
        private readonly Neutronium.WPF.Internal.HTMLControlBase _HTMLControlBase;
        private readonly IWPFWebWindow _IWPFWebWindow;

        public IWebBrowserWindow HTMLWindow => _IWPFWebWindow.HTMLWindow;
        public IWPFWebWindow IWPFWebWindow => _IWPFWebWindow;
        public IDispatcher UIDispatcher => new WPFUIDispatcher(_UIElement.Dispatcher);

        public WPFHTMLWindowProvider(IWPFWebWindow iIWPFWebWindow, Neutronium.WPF.Internal.HTMLControlBase iHTMLControlBase)
        {
            _IWPFWebWindow = iIWPFWebWindow;
            _HTMLControlBase = iHTMLControlBase;
            _UIElement = _IWPFWebWindow.UIElement;
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
            return _IWPFWebWindow.OnDebugToolsRequest();
        }

        public void CloseDebugTools() 
        {
            _IWPFWebWindow.CloseDebugTools();
        }

        public void Dispose()
        {
            _UIElement.Visibility = Visibility.Hidden;
            _HTMLControlBase.MainGrid.Children.Remove(_UIElement);

            _IWPFWebWindow.Dispose();
        }
    }
}
