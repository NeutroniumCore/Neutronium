using Awesomium.Core;
using Awesomium.Windows.Controls;
using HTML_WPF.Component;
using MVVM.HTML.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MVVM.Awesomium
{
    internal class AwesomiumWPFWebWindow : IWPFWebWindow
    {

        private WebSession _Session;
        private WebControl _WebControl;

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

            _AwesomiumHTMLWindow = new AwesomiumHTMLWindow(_Session, _WebControl);     
        }

        private AwesomiumHTMLWindow _AwesomiumHTMLWindow;


        public IHTMLWindow IHTMLWindow
        {
            get { return _AwesomiumHTMLWindow; }
        }

        public void Inject(Key KeyToInject)
        {
            IWebView wv = _WebControl;
            KeyEventArgs kev = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, KeyToInject);
            wv.InjectKeyboardEvent(kev.GetKeyboardEvent(WebKeyboardEventType.KeyDown));
        }

        public UIElement UIElement
        {
            get { return _WebControl; }
        }

        public void Dispose()
        {
            _WebControl.Dispose();
            _AwesomiumHTMLWindow.Dispose();
        }
    }
}
