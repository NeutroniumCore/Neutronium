using System;
using System.Threading.Tasks;
using CefGlue.TestInfra.CefWindowless;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;
using Xilium.CefGlue;

namespace CefGlue.TestInfra
{
    internal class CefGlueWindowlessSharedHtmlEngine :  IWindowlessHTMLEngine
    {
        private TestCefGlueHTMLWindowProvider _TestCefGlueHTMLWindowProvider;
        private CefFrame _CefFrame;
        private CefBrowser _CefBrowser;

        public IWebView WebView { get; private set; }
        public IWebBrowserWindow HTMLWindow => _TestCefGlueHTMLWindowProvider.HtmlWindow;
        public IWebBrowserWindowProvider HTMLWindowProvider => _TestCefGlueHTMLWindowProvider;

        public void Init(string path, IWebSessionLogger logger)
        {
            var cc = InitTask(path, logger).Result;
            WebView = cc.HtmlWindow.MainFrame;
        }

        private Task<IWebBrowserWindowProvider> InitTask(string fullpath, IWebSessionLogger logger)
        {
            TaskCompletionSource<IWebBrowserWindowProvider> tcs = new TaskCompletionSource<IWebBrowserWindowProvider>();
            Task.Run(async () =>
            {
                var cefWindowInfo = CefWindowInfo.Create();
                cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

                //// Settings for the browser window itself (e.g. enable JavaScript?).
                var cefBrowserSettings = new CefBrowserSettings();

                // Initialize some the cust interactions with the browser process.
                var cefClient = new TestCefClient();

                // Start up the browser instance.
                CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, fullpath);

                _CefBrowser = await cefClient.GetLoadedBrowserAsync();

                _CefFrame = _CefBrowser.GetMainFrame();
                _TestCefGlueHTMLWindowProvider = new TestCefGlueHTMLWindowProvider(_CefFrame, cefClient);
                tcs.SetResult(_TestCefGlueHTMLWindowProvider);
            });

            return tcs.Task;
        }

        public void Dispose()
        {
            var browserhost = _CefBrowser.GetHost();
            browserhost.CloseBrowser(true);
            _CefFrame.Dispose();
            _CefBrowser.Dispose();
        }
    }
}
