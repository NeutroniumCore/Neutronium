using System;
using System.Threading.Tasks;
using CefGlue.TestInfra.CefWindowless;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using MVVM.HTML.Core.JavascriptUIFramework;
using Xilium.CefGlue;

namespace CefGlue.TestInfra
{
    internal class CefGlueWindowlessSharedJavascriptEngine :  IWindowlessJavascriptEngine
    {
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;
        private TestCefGlueHTMLWindowProvider _TestCefGlueHTMLWindowProvider;
        private CefFrame _CefFrame;
        private CefBrowser _CefBrowser;

        public IWebView WebView { get; private set; }
        public HTMLViewEngine ViewEngine { get; private set; }
        public IHTMLWindow HTMLWindow => _TestCefGlueHTMLWindowProvider.HTMLWindow;

        public CefGlueWindowlessSharedJavascriptEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
        {
            _JavascriptUIFrameworkManager = javascriptUIFrameworkManager;
        }

        public void Init(string path, IWebSessionLogger logger)
        {
            var cc = InitTask(path, logger).Result;
            WebView = cc.HTMLWindow.MainFrame;
        }

        private Task<IHTMLWindowProvider> InitTask(string fullpath, IWebSessionLogger logger)
        {
            TaskCompletionSource<IHTMLWindowProvider> tcs = new TaskCompletionSource<IHTMLWindowProvider>();
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
                ViewEngine = new HTMLViewEngine(_TestCefGlueHTMLWindowProvider,  _JavascriptUIFrameworkManager, logger);
                tcs.SetResult(_TestCefGlueHTMLWindowProvider);
            });

            return tcs.Task;
        }

        public void Dispose()
        {
            _CefFrame.Dispose();
            _CefBrowser.Dispose();
        }
    }
}
