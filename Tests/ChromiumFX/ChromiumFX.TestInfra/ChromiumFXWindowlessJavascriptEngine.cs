using System;
using System.Reflection;
using System.Threading.Tasks;
using Chromium;
using Chromium.Remote;
using ChromiumFX.TestInfra.Helper;
using HTMEngine.ChromiumFX.EngineBinding;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace ChromiumFX.TestInfra
{
    public class ChromiumFXWindowlessJavascriptEngine : IWindowlessJavascriptEngine 
    {
        private readonly IJavascriptUIFrameworkManager _FrameWork;
        private readonly WpfThread _WpfThread;
        private CfxClient _CfxClient;
        private CfrFrame _CfrFrame;
        private readonly Task<RenderProcessHandler> _RenderProcessHandler;
        private CfrBrowser _CfrBrowser;
        private CfrApp _CfrApp;

        public HTMLViewEngine ViewEngine { get; private set;  }
        public IWebView WebView { get; private set; }

        public ChromiumFXWindowlessJavascriptEngine(WpfThread wpfThread, Task<RenderProcessHandler> renderProcessHandler, IJavascriptUIFrameworkManager frameWork) 
        {
            _FrameWork = frameWork;
            _RenderProcessHandler = renderProcessHandler;
            _WpfThread = wpfThread;
        }

        public void Init(string path = "javascript\\index.html") 
        {
            path = path ?? "javascript\\index.html";
            path = String.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), path);
            InitAsync(path).Wait();
        }

        private async Task InitAsync(string path) 
        {
            var taskload = _WpfThread.Dispatcher.Invoke(() => RawInit(path));
            var processehandler = await _RenderProcessHandler;
            WebView = await GetFrame(processehandler);
            _CfrApp = processehandler.App;
            await taskload;

            ViewEngine = new HTMLViewEngine(new ChromiumFXHTMLWindowProvider(WebView, new Uri(path)), _FrameWork);
        }

        private CfxBrowserSettings GetSettings()
        {
            return new CfxBrowserSettings();
        }

        private Task<ChromiumFXWebView> GetFrame(RenderProcessHandler renderProcessHandler)
        {
            var tcs = new TaskCompletionSource<ChromiumFXWebView>();
            renderProcessHandler.OnNewFrame += (e) => 
            {
                _CfrFrame = e.Frame;
                _CfrBrowser = e.Browser;
                tcs.SetResult(new ChromiumFXWebView(_CfrBrowser));
            };
            return tcs.Task;
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
                //_CfxFrame = args.Frame;
                //_CfxBrowser = args.Browser;
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
            _CfrFrame.Dispose();
            _CfrBrowser.Dispose();
            _CfrApp.Dispose();
        }
    }
}
