using System;
using System.Threading.Tasks;
using Chromium;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.Control;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding;
using Tests.Infra.WebBrowserEngineTesterHelper.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.Windowless;

namespace Tests.ChromiumFX.Infra 
{
    public class ChromiumFxWindowlessHtmlEngine : IWindowlessHTMLEngine 
    {
        private readonly WpfThread _WpfThread;
        private CfxClient _CfxClient;
        private readonly Task<ChromiumFxWebView> _ChromiumFXWebViewTask;
        private CfxBrowser _CfxBrowser;
        private ChromiumFXHTMLWindowProvider _ChromiumFXHTMLWindowProvider;

        public IWebView WebView { get; private set; }
        public IWebBrowserWindow HTMLWindow => _ChromiumFXHTMLWindowProvider.HtmlWindow;
        public IWebBrowserWindowProvider HTMLWindowProvider => _ChromiumFXHTMLWindowProvider;

        internal ChromiumFxWindowlessHtmlEngine(WpfThread wpfThread, Task<ChromiumFxWebView> chromiumFxWebViewTask) 
        {
            _ChromiumFXWebViewTask = chromiumFxWebViewTask;
            _WpfThread = wpfThread;
        }

        public void Init(string path, IWebSessionLogger logger) 
        {
            InitAsync(path, logger).Wait();
        }

        private async Task InitAsync(string path, IWebSessionLogger logger ) 
        {
            var taskLoad = _WpfThread.Dispatcher.Invoke(() => RawInit(path));      
            WebView = await _ChromiumFXWebViewTask;
            _CfxBrowser = await taskLoad;
            _ChromiumFXHTMLWindowProvider = new ChromiumFXHTMLWindowProvider(_CfxClient, WebView, new Uri(path));
        }

        private CfxBrowserSettings GetSettings()
        {
            return new CfxBrowserSettings();
        }

        private Task<CfxBrowser> RawInit(string path) 
        {  
            var loadTaskCompletionSource = new TaskCompletionSource<CfxBrowser>();
            var cfxWindowInfo = new CfxWindowInfo();
            _CfxClient = new CfxClient();

            var loadHandler = new CfxLoadHandler();
            loadHandler.OnLoadEnd += (sender, args) => 
            {
                loadTaskCompletionSource.TrySetResult(args.Browser);
            };
            _CfxClient.GetLoadHandler += (o, e) => e.SetReturnValue(loadHandler); 

            var lifeSpanHandler = new CfxLifeSpanHandler();
            _CfxClient.GetLifeSpanHandler += (o, e) => e.SetReturnValue(lifeSpanHandler);

            var renderHandler = new CfxRenderHandler();
            _CfxClient.GetRenderHandler += (sender, e) => e.SetReturnValue(renderHandler);

            if (!CfxBrowserHost.CreateBrowser(cfxWindowInfo, _CfxClient, path, GetSettings(), null, null))
                throw new Exception("Problem initializing CEF");

            return loadTaskCompletionSource.Task;
        }

        public void Dispose() 
        {
            var browserHost = _CfxBrowser.Host;
            browserHost.CloseBrowser(true);
            _CfxBrowser.Dispose();
            _CfxClient.Dispose();
        }
    }
}
