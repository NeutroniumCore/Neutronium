using Awesomium.Core;
using IntegratedTest;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            TaskCompletionSource<SynchronizationContext> tcs = new TaskCompletionSource<SynchronizationContext>();
            Task.Factory.StartNew(() =>
            {
                WebCore.Initialize(new WebConfig());
                WebSession session = WebCore.CreateWebSession(WebPreferences.Default);

                var webView = WebCore.CreateWebView(500, 500);
                ipath = ipath ?? "javascript\\index.html";
                webView.Source = new Uri(string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), ipath));

                WebCore.Started += (o, e) => 
                {
                    WebView = new AwesomiumWebView(webView);
                    var htmlWindowProvider = new AwesomiumTestHTMLWindowProvider(WebView, ipath);
                    ViewEngine = new HTMLViewEngine(
                        htmlWindowProvider,
                        _JavascriptUIFrameworkManager
                    );
                    tcs.SetResult(SynchronizationContext.Current); 
                };

                while (webView.IsLoading)
                {
                    WebCore.Run();
                }

                

                _EndTaskCompletionSource.SetResult(null);
            } );

            return tcs.Task;
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
