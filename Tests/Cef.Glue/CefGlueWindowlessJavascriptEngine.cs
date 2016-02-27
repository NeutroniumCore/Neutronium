using KnockoutUIFramework;
using MVVM.Cef.Glue.CefSession;
using MVVM.Cef.Glue.Test.CefWindowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using MVVM.HTML.Core.Infra;
using IntegratedTest;

namespace MVVM.Cef.Glue.Test.Generic
{
    internal class CefGlueWindowlessJavascriptEngine : IDisposable, IWindowlessJavascriptEngine
    {

        public CefGlueWindowlessJavascriptEngine()
        { 
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
                        new KnockoutUiFrameworkManager()
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
