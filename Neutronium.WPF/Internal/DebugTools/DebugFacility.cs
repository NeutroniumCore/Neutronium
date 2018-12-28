using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WPF.Internal.DebugTools
{
    internal class DebugFacility: IDebugFacility
    {
        private readonly ICompleteWebViewComponent _WebViewComponent;
        private readonly Action<string, int, int, Func<IWebView, IDisposable>> _ShowWindow;

        public DebugFacility(ICompleteWebViewComponent webViewComponent)
        {
            _WebViewComponent = webViewComponent;
        }

        public void RunJavascript(string code)
        {
            _WebViewComponent.ExecuteJavascript(code);
        }

        public void OpenNewWindow(string path, int width, int height, Func<IWebView, IWebView, IDisposable> injectCode)
        {
            var mainFrame = _WebViewComponent.HTMLWindow.MainFrame;
            _WebViewComponent.ShowHtmlWindow(path, width, height, debug => injectCode(mainFrame, debug));
        }
    }
}
