using System;
using System.Reflection;
using System.Threading.Tasks;
using IntegratedTest;
using MVVM.Cef.Glue.CefSession;
using MVVM.Cef.Glue.Test.CefWindowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.Tests.Infra
{
    internal class CefGlueWindowlessJavascriptEngine : IDisposable, IWindowlessJavascriptEngine
    {
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;

        public CefGlueWindowlessJavascriptEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
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
            Task.Run(() =>
            {
                CefCoreSessionSingleton.GetAndInitIfNeeded();

                CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
                cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

                // Settings for the browser window itself (e.g. enable JavaScript?).
                var cefBrowserSettings = new CefBrowserSettings();

                // Initialize some the cust interactions with the browser process.
                var cefClient = new TestCefClient();

                ipath = ipath ?? "javascript\\index.html";
                string fullpath = string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), ipath);

                // Start up the browser instance.
                CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, fullpath);

                cefClient.GetLoadedBroserAsync().ContinueWith(t =>
                {
                    var frame = t.Result.GetMainFrame();
                    var htmlWindowProvider = new TestCefGlueHTMLWindowProvider(frame);
                    ViewEngine = new HTMLViewEngine(
                        htmlWindowProvider,
                        _JavascriptUIFrameworkManager
                    );
                    tcs.SetResult(htmlWindowProvider);
                }
                );
            });

            return tcs.Task;
        }

        public void Dispose()
        {
            CefCoreSessionSingleton.Clean();
        }
    }
}
