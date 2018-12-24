using System;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.Navigation;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WPF.Internal.DebugTools
{
    internal class DebugFacility: IDebugFacility
    {
        private readonly DoubleBrowserNavigator _WpfDoubleBrowserNavigator;
        private readonly Action<string, int, int, Func<IWebView, IDisposable>> _ShowWindow;

        public DebugFacility(DoubleBrowserNavigator wpfDoubleBrowserNavigator, Action<string, int, int, Func<IWebView, IDisposable>> showWindow)
        {
            _WpfDoubleBrowserNavigator = wpfDoubleBrowserNavigator;
            _ShowWindow = showWindow;
        }

        public void RunJavascript(string code)
        {
            _WpfDoubleBrowserNavigator.ExcecuteJavascript(code);
        }

        public void OpenNewWindow(string path, int width, int height, Func<IWebView, IWebView, IDisposable> injectCode)
        {
            _ShowWindow(path, width, height, debug => injectCode(_WpfDoubleBrowserNavigator.HTMLWindow.MainFrame, debug));
        }
    }
}
