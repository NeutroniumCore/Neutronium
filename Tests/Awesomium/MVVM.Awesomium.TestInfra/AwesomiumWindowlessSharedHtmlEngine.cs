using System;
using System.Threading.Tasks;
using Awesomium.Core;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.Awesomium.Engine;
using Neutronium.WebBrowserEngine.Awesomium.Internal;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;

namespace Tests.Awesomium.Infra 
{
    internal class AwesomiumWindowlessSharedHtmlEngine : IWindowlessHTMLEngine
    {
        private IWebView _CurrentWebView;
        private AwesomiumTestHTMLWindowProvider _AwesomiumTestHTMLWindowProvider;

        public Neutronium.Core.WebBrowserEngine.JavascriptObject.IWebView WebView { get; private set; }
        public IWebBrowserWindow HTMLWindow => _AwesomiumTestHTMLWindowProvider.HTMLWindow;
        public IWebBrowserWindowProvider HTMLWindowProvider => _AwesomiumTestHTMLWindowProvider;

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
