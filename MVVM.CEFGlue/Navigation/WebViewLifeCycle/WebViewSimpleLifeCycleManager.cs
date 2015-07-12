//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using Xilium.CefGlue;
//using Xilium.CefGlue.WPF;

//namespace MVVM.CEFGlue.Navigation
//{
//    public class WebViewSimpleLifeCycleManager : IWebViewLifeCycleManager
//    {
//        private WpfCefBrowser _First;
//        private WpfCefBrowser _Second;
//        private bool _FirstElement = true;
//        public WebViewSimpleLifeCycleManager(WpfCefBrowser First, WpfCefBrowser Second)
//        {
//            _First=First;
//            _Second = Second;
//            _First.Visibility = Visibility.Hidden;
//            _Second.Visibility = Visibility.Hidden;
//        }

//        public CefV8Context Create()
//        {      
//            var res = _FirstElement ? _First : _Second;
//            _FirstElement = !_FirstElement;
//            if (res.WebSession!=null)
//                res.WebSession.ClearCache();
//            return res;
//        }

//        public void Dispose(WpfCefBrowser ioldwebview)
//        { 
//            ioldwebview.Visibility = Visibility.Hidden;
//            ioldwebview.NavigateTo("about:blank");
//        }


//        public void Display(WpfCefBrowser webview)
//        {
//            webview.Visibility = Visibility.Visible;
//        }
//    }
//}
