using System;
using System.Reflection;
using System.Threading.Tasks;
using IntegratedTest;
using MVVM.Cef.Glue.Test.CefWindowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Tests.Infra
{
    internal class CefGlueWindowlessSharedJavascriptEngine :  IWindowlessJavascriptEngine
    {
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;
        private CefFrame _CefFrame;
        private CefBrowser _CefBrowser;

        public CefGlueWindowlessSharedJavascriptEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
        {
            _JavascriptUIFrameworkManager = javascriptUIFrameworkManager;
        }

        public void Init(string path = "javascript\\index.html")
        {
            var cc = InitTask(path).Result;
            WebView = cc.HTMLWindow.MainFrame;
        }

        public IWebView WebView { get; private set; }

        public HTMLViewEngine ViewEngine { get; private set; }

        private Task<IHTMLWindowProvider> InitTask(string ipath)
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

                ipath = ipath ?? "javascript\\index.html";
                string fullpath = string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), ipath);

                // Start up the browser instance.
                CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, fullpath);

                _CefBrowser = await cefClient.GetLoadedBroserAsync();

                _CefFrame = _CefBrowser.GetMainFrame();
                var htmlWindowProvider = new TestCefGlueHTMLWindowProvider(_CefFrame);
                ViewEngine = new HTMLViewEngine(
                    htmlWindowProvider,
                    _JavascriptUIFrameworkManager);
                tcs.SetResult(htmlWindowProvider);
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
