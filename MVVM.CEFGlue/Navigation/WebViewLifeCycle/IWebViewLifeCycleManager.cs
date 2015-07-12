using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace MVVM.CEFGlue.Navigation
{
    public interface IWebViewLifeCycleManager
    {
        WpfCefBrowser Create();

        void Display(WpfCefBrowser webview);

        void Dispose(WpfCefBrowser ioldwebview);
    }
}
