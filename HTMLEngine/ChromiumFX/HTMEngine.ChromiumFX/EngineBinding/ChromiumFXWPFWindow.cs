using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chromium;
using Chromium.WebBrowser;
using HTMEngine.ChromiumFX.WPF;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTMEngine.ChromiumFX.EngineBinding 
{
    internal class ChromiumFXWPFWindow : IWPFWebWindow 
    {
        private readonly ChromiumFxControl _ChromiumFxControl;
        private readonly ChromiumWebBrowser _ChromiumWebBrowser;
        private readonly ChromiumFxControlHTMLWindow _ChromiumFxControlHTMLWindow;

        public ChromiumFXWPFWindow()
        {
            _ChromiumFxControl = new ChromiumFxControl()
            {
                Visibility = Visibility.Hidden,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };
            _ChromiumWebBrowser = _ChromiumFxControl.ChromiumWebBrowser;
            var dispatcher = new WPFUIDispatcher(_ChromiumFxControl.Dispatcher);
            _ChromiumFxControlHTMLWindow = new ChromiumFxControlHTMLWindow(_ChromiumWebBrowser, dispatcher);
        }

        public IHTMLWindow HTMLWindow 
        {
            get { return _ChromiumFxControlHTMLWindow; }
        }

        public void Inject(Key keyToInject) 
        {
            var cxKeyEvent = new CfxKeyEvent() 
            {
                WindowsKeyCode = (int) keyToInject
            };

            _ChromiumWebBrowser.Browser.Host.SendKeyEvent(cxKeyEvent);
        }

        public UIElement UIElement
        {
            get { return _ChromiumFxControl; }
        }

        public bool IsUIElementAlwaysTopMost 
        {
            get { return true; }
        }

        public void Dispose()
        {
            _ChromiumWebBrowser.Dispose();
        }
    }
}
