using Awesomium.Core;
using IntegratedTest;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MVVM.Awesomium.Tests
{
    internal class AwesomiumWindowlessJavascriptEngine : IWindowlessJavascriptEngine
    {
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;
        private readonly TaskCompletionSource<object> _EndTaskCompletionSource;

        public AwesomiumWindowlessJavascriptEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
        {
            _JavascriptUIFrameworkManager = javascriptUIFrameworkManager;
            _EndTaskCompletionSource = new TaskCompletionSource<object>();
        }

        public void Init(string path = "javascript\\index.html")
        {
            InitAsync(path).Wait();
        }

        private Task InitAsync(string ipath = "javascript\\index.html")
        {
            var taskLoaded = new TaskCompletionSource<object>();
            var taskContextLoaded = new TaskCompletionSource<SynchronizationContext>();
            Task.Factory.StartNew(() =>
            {
                WebCore.Initialize(new WebConfig());
                WebSession session = WebCore.CreateWebSession(WebPreferences.Default);

                var webView = WebCore.CreateWebView(500, 500);
                ipath = ipath ?? "javascript\\index.html";
                webView.Source = new Uri(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), ipath));

                WebCore.ShuttingDown += (o, e) => 
                {
                    if (e.Exception != null)
                    {
                        Console.WriteLine("Exception on main thread {0}",e.Exception);
                        e.Cancel = true;
                    }
                };

                WebCore.Started += (o, e) => 
                {
                    AwesomiumWPFWebWindowFactory.WebCoreThread = Thread.CurrentThread;
                    WebView = new AwesomiumWebView(webView);
                    var htmlWindowProvider = new AwesomiumTestHTMLWindowProvider(WebView, ipath);
                    ViewEngine = new HTMLViewEngine(
                        htmlWindowProvider,
                        _JavascriptUIFrameworkManager
                    );
                    taskContextLoaded.SetResult(SynchronizationContext.Current); 
                };

                var viewReadyExecuter = new ViewReadyExecuter(webView, () => { taskLoaded.TrySetResult(null); });
                viewReadyExecuter.Do();

                while (webView.IsLoading)
                {
                    WebCore.Run();
                }

                _EndTaskCompletionSource.SetResult(null);
            } );

            return Task.WhenAll(taskLoaded.Task, taskContextLoaded.Task) ;
        }

        public HTMLViewEngine ViewEngine
        {
            get; private set;
        }
 
        public void Dispose()
        {
            WebCore.Shutdown();
            _EndTaskCompletionSource.Task.Wait();
        }

        public HTML.Core.JavascriptEngine.JavascriptObject.IWebView WebView
        {
            get; private set;
        }
    }
}
