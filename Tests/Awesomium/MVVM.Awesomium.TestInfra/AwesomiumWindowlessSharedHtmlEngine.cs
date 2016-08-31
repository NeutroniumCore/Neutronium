using System;
using System.Threading.Tasks;
using Awesomium.Core;
using HTMLEngine.Awesomium.HTMLEngine;
using HTMLEngine.Awesomium.Internal;
using MVVM.HTML.Core;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace MVVM.Awesomium.TestInfra
{
    internal class AwesomiumWindowlessSharedHtmlEngine : IWindowlessHTMLEngine
    {
        private IWebView _CurrentWebView;
        private AwesomiumTestHTMLWindowProvider _AwesomiumTestHTMLWindowProvider;

        public HTML.Core.JavascriptEngine.JavascriptObject.IWebView WebView { get; private set; }
        public IHTMLWindow HTMLWindow => _AwesomiumTestHTMLWindowProvider.HTMLWindow;
        public IHTMLWindowProvider HTMLWindowProvider => _AwesomiumTestHTMLWindowProvider;

        public void Init(string path, IWebSessionLogger logger)
        {
            InitAsync(path, logger).Wait();
        }

        private async Task InitAsync(string ipath, IWebSessionLogger logger)
        {
            var taskLoaded = new TaskCompletionSource<object>();

            WebCore.QueueWork( () => 
            {
                _CurrentWebView = WebCore.CreateWebView(500, 500);
                var uri = new Uri(ipath);
                _CurrentWebView.Source = uri;
                WebView = new AwesomiumWebView(_CurrentWebView);
                _AwesomiumTestHTMLWindowProvider = new AwesomiumTestHTMLWindowProvider(WebView, uri);
                var viewReadyExecuter = new ViewReadyExecuter(_CurrentWebView, () => { taskLoaded.TrySetResult(null); });
                viewReadyExecuter.Do();
            });

            await taskLoaded.Task;
        }      
 
        public void Dispose()
        {
            WebCore.QueueWork(_CurrentWebView, () => _CurrentWebView.Dispose());      
        }
    }
}
