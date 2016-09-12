using System;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.WindowImplementation
{
    internal sealed class WpfCefDisplayHandler : CefDisplayHandler
    {
        private readonly WpfCefBrowser _owner;

        public WpfCefDisplayHandler(WpfCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
        } 

        protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
        }

        protected override void OnTitleChange(CefBrowser browser, string title)
        {
        }

        protected override void OnStatusMessage(CefBrowser browser, string value)
        {
        }

        protected override bool OnConsoleMessage(CefBrowser browser, string message, string source, int line)
        {
            return _owner.OnConsoleMessage(browser, message, source, line);
        }
    }
}
