using System;
using System.Threading.Tasks;
using Chromium;
using HTMEngine.ChromiumFX.EngineBinding;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace ChromiumFX.TestInfra
{
    public class ChromiumFXWindowlessJavascriptEngine : IWindowlessJavascriptEngine 
    {
        private readonly IJavascriptUIFrameworkManager _FrameWork;
        private readonly WpfThread _WpfThread;
        private CfxClient _CfxClient;
        private readonly Task<ChromiumFXWebView> _ChromiumFXWebViewTask;
        private CfxBrowser _CfxBrowser;

        public HTMLViewEngine ViewEngine { get; private set;  }
        public IWebView WebView { get; private set; }

        internal ChromiumFXWindowlessJavascriptEngine(WpfThread wpfThread, Task<ChromiumFXWebView> chromiumFxWebViewTask, IJavascriptUIFrameworkManager frameWork) 
        {
            _FrameWork = frameWork;
            _ChromiumFXWebViewTask = chromiumFxWebViewTask;
            _WpfThread = wpfThread;
        }

        public void Init(string path, IWebSessionLogger logger) 
        {
            InitAsync(path, logger).Wait();
        }

        private async Task InitAsync(string path, IWebSessionLogger logger ) 
        {
            var taskload = _WpfThread.Dispatcher.Invoke(() => RawInit(path));      
            WebView = await _ChromiumFXWebViewTask;
            await taskload;

            ViewEngine = new HTMLViewEngine(new ChromiumFXHTMLWindowProvider(WebView, new Uri(path)), _FrameWork, logger);
        }

        private CfxBrowserSettings GetSettings()
        {
            return new CfxBrowserSettings();
        }

        private Task RawInit(string path) 
        {  
            var loadTaskCompletionSource = new TaskCompletionSource<int>();
            var cfxWindowInfo = new CfxWindowInfo();
            cfxWindowInfo.SetAsWindowless(true);

            _CfxClient = new CfxClient();

            var loadHandler = new CfxLoadHandler();
            loadHandler.OnLoadEnd += (sender, args) => 
            {
                _CfxBrowser = args.Browser;
                loadTaskCompletionSource.TrySetResult(0);
            };
            _CfxClient.GetLoadHandler += (o, e) => e.SetReturnValue(loadHandler); 

            var lifeSpanHandler = new CfxLifeSpanHandler();
            _CfxClient.GetLifeSpanHandler += (o, e) => e.SetReturnValue(lifeSpanHandler);

            var renderHandler = new CfxRenderHandler();
            _CfxClient.GetRenderHandler += (sender, e) => e.SetReturnValue(renderHandler);

            if (!CfxBrowserHost.CreateBrowser(cfxWindowInfo, _CfxClient, path, GetSettings(), null))
                throw new Exception("Problem initializing CEF");

            return loadTaskCompletionSource.Task;
        }

        public void Dispose() 
        {
            var browserhost = _CfxBrowser.Host;
            browserhost.CloseBrowser(true);
        }
    }
}
