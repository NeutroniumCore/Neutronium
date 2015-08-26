
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.Window;

using MVVM.Cef.Glue.WPF;


namespace MVVM.Cef.Glue.Navigation
{
    public class CefGlueHTMLWindowProvider : IHTMLWindowProvider
    {
        private WpfCefBrowser _WpfCefBrowser;
        private HTMLControlBase _HTMLControlBase;

        public CefGlueHTMLWindowProvider(WpfCefBrowser iWpfCefBrowser, HTMLControlBase iHTMLControlBase)
        {
            _WpfCefBrowser = iWpfCefBrowser;
            _HTMLControlBase = iHTMLControlBase;
        }

        public IHTMLWindow HTMLWindow
        {
            get { return _WpfCefBrowser; }
        }

        public void Show()
        {
            _WpfCefBrowser.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _WpfCefBrowser.Visibility = Visibility.Hidden;
            
        }

        public void Dispose()
        {
            _WpfCefBrowser.Visibility = Visibility.Hidden;
            _HTMLControlBase.MainGrid.Children.Remove(_WpfCefBrowser);
            _WpfCefBrowser.Dispose();
        }


        public IDispatcher UIDispatcher
        {
            get { return _WpfCefBrowser.GetDispatcher(); }
        }
    }
}
