using Awesomium.Core;
using Awesomium.Windows.Controls;
using HTML_WPF.Component;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.Awesomium
{
    internal class AwesomiumWPFWebWindow : IWPFWebWindow
    {
        private readonly WebSession _Session;
        private readonly WebControl _WebControl;
        private readonly AwesomiumHTMLWindow _AwesomiumHTMLWindow;

        public AwesomiumWPFWebWindow(WebSession iSession)
        {
            _Session = iSession;

            _WebControl = new WebControl()
            {
                WebSession = _Session,
                Visibility = Visibility.Hidden,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                ContextMenu = new ContextMenu() { Visibility = Visibility.Collapsed }
            };

            _AwesomiumHTMLWindow = new AwesomiumHTMLWindow(_WebControl);     
        }

        public IHTMLWindow HTMLWindow
        {
            get { return _AwesomiumHTMLWindow; }
        }
        public UIElement UIElement
        {
            get { return _WebControl; }
        }

        public void Inject(Key KeyToInject)
        {
            IWebView wv = _WebControl;
            KeyEventArgs kev = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, KeyToInject);
            wv.InjectKeyboardEvent(kev.GetKeyboardEvent(WebKeyboardEventType.KeyDown));
        }

        public void Dispose()
        {
            _WebControl.Dispose();
            _AwesomiumHTMLWindow.Dispose();
        }
    }
}
