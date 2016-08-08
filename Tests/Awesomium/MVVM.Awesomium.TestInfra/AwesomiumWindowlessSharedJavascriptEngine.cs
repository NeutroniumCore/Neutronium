using System;
using System.Reflection;
using System.Threading.Tasks;
using Awesomium.Core;
using HTMLEngine.Awesomium.HTMLEngine;
using HTMLEngine.Awesomium.Internal;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Awesomium.TestInfra 
{
    internal class AwesomiumWindowlessSharedJavascriptEngine : IWindowlessJavascriptEngine
    {
        private readonly IJavascriptUIFrameworkManager _JavascriptUIFrameworkManager;
        private readonly TaskCompletionSource<object> _EndTaskCompletionSource;
        private IWebView _CurrentWebView;

        public AwesomiumWindowlessSharedJavascriptEngine(IJavascriptUIFrameworkManager javascriptUIFrameworkManager)
        {
            _JavascriptUIFrameworkManager = javascriptUIFrameworkManager;
            _EndTaskCompletionSource = new TaskCompletionSource<object>();
        }

        public void Init(string path = "javascript\\index.html")
        {
            InitAsync(path).Wait();
        }

        private async Task InitAsync(string ipath = "javascript\\index.html")
        {
            var taskLoaded = new TaskCompletionSource<object>();

            WebCore.QueueWork( () => 
            {
                _CurrentWebView = WebCore.CreateWebView(500, 500);
                ipath = ipath ?? "javascript\\index.html";
                var uri = new Uri($"{Assembly.GetExecutingAssembly().GetPath()}\\{ipath}");
                _CurrentWebView.Source = uri;
                WebView = new AwesomiumWebView(_CurrentWebView);
                var htmlWindowProvider = new AwesomiumTestHTMLWindowProvider(WebView, uri);
                ViewEngine = new HTMLViewEngine(
                    htmlWindowProvider,
                    _JavascriptUIFrameworkManager
                );
                var viewReadyExecuter = new ViewReadyExecuter(_CurrentWebView, () => { taskLoaded.TrySetResult(null); });
                viewReadyExecuter.Do();
            });

            await taskLoaded.Task;
        }

        public HTMLViewEngine ViewEngine
        {
            get; private set;
        }
 
        public void Dispose()
        {
            WebCore.QueueWork(_CurrentWebView, () => _CurrentWebView.Dispose());      
        }

        public HTML.Core.JavascriptEngine.JavascriptObject.IWebView WebView
        {
            get; private set;
        }
    }
}
